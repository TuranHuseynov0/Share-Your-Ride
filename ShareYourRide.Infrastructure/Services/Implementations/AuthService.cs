using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ShareYourRide.Application.DTOs.Auth;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Identity;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IDistributedCache _cache;

        private static readonly DistributedCacheEntryOptions PendingCacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            ISmsSender smsSender,
            IEmailSender emailSender,
            IDistributedCache cache)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _smsSender = smsSender;
            _emailSender = emailSender;
            _cache = cache;
        }

        private static string PendingKey(Guid id) => $"pending-registration:{id}";
        private static string GenerateOtp() => Random.Shared.Next(100000, 999999).ToString();

        // ---------- Redis üzərində pending registration idarəsi ----------

        private async Task SetPendingAsync(Guid id, PendingRegistration pending)
        {
            var json = JsonSerializer.Serialize(pending, JsonOptions);
            await _cache.SetStringAsync(PendingKey(id), json, PendingCacheOptions);
        }

        private async Task<PendingRegistration?> GetPendingAsync(Guid id)
        {
            var json = await _cache.GetStringAsync(PendingKey(id));
            return json == null ? null : JsonSerializer.Deserialize<PendingRegistration>(json, JsonOptions);
        }

        private async Task RemovePendingAsync(Guid id) => await _cache.RemoveAsync(PendingKey(id));

        // ---------- Qeydiyyat: 1-ci addım (şəxsi məlumatlar) ----------

        public async Task<RegisterPersonalInfoResponseDto> RegisterPersonalInfoAsync(RegisterPersonalInfoDto dto)
        {
            var finExists = (await _unitOfWork.Users.FindAsync(u => u.FinCode == dto.FinCode)).Any();
            if (finExists)
                throw new InvalidOperationException("Bu FIN kod artıq qeydiyyatdan keçib.");

            var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (phoneExists)
                throw new InvalidOperationException("Bu telefon nömrəsi artıq qeydiyyatdan keçib.");

            var emailExists = await _userManager.FindByEmailAsync(dto.Email) != null;
            if (emailExists)
                throw new InvalidOperationException("Bu email artıq qeydiyyatdan keçib.");

            var pendingId = Guid.NewGuid();
            var pending = new PendingRegistration
            {
                PersonalInfo = dto,
                OtpCode = GenerateOtp()
            };

            await SetPendingAsync(pendingId, pending);
            await _emailSender.SendAsync(dto.Email, "Qeydiyyat təsdiq kodu", $"Kodunuz: {pending.OtpCode}");

            return new RegisterPersonalInfoResponseDto
            {
                UserId = pendingId,
                MaskedEmail = MaskEmail(dto.Email),
                OtpExpirySeconds = 600,
                RequiresVehicleInfo = dto.Role == TrajectoryRole.Driver
            };
        }

        // ---------- Qeydiyyat: 2-ci addım (maşın məlumatları, yalnız Driver üçün) ----------

        public async Task RegisterVehicleInfoAsync(RegisterVehicleDto dto)
        {
            var pending = await GetPendingAsync(dto.UserId)
                ?? throw new InvalidOperationException("Qeydiyyat sessiyasının vaxtı bitib. Zəhmət olmasa yenidən başlayın.");

            pending.VehicleInfo = dto;
            await SetPendingAsync(dto.UserId, pending);
        }

        // ---------- OTP təsdiqi: DB-yə yazılan yeganə yer ----------

        public async Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto)
        {
            var pending = await GetPendingAsync(dto.UserId)
                ?? throw new InvalidOperationException("Qeydiyyat sessiyasının vaxtı bitib. Zəhmət olmasa yenidən başlayın.");

            if (pending.OtpCode != dto.Code)
            {
                pending.FailedAttempts++;
                if (pending.FailedAttempts >= 5)
                {
                    await RemovePendingAsync(dto.UserId);
                    throw new InvalidOperationException("Cəhd limiti aşıldı. Zəhmət olmasa yenidən qeydiyyatdan keçin.");
                }
                await SetPendingAsync(dto.UserId, pending);
                throw new InvalidOperationException("Kod yanlışdır və ya vaxtı bitib.");
            }

            var info = pending.PersonalInfo;

            // Commit anında təkrar unikallıq yoxlaması (race condition qarşısını almaq üçün)
            var finExists = (await _unitOfWork.Users.FindAsync(u => u.FinCode == info.FinCode)).Any();
            if (finExists)
            {
                await RemovePendingAsync(dto.UserId);
                throw new InvalidOperationException("Bu FIN kod artıq qeydiyyatdan keçib.");
            }

            var appUser = new ApplicationUser
            {
                UserName = info.Email,
                Email = info.Email,
                PhoneNumber = info.PhoneNumber,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(appUser, info.Password);
            if (!identityResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", identityResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(appUser, nameof(RoleType.User));

            var domainUser = new User
            {
                ApplicationUserId = appUser.Id,
                FirstName = info.FirstName,
                LastName = info.LastName,
                FinCode = info.FinCode,
                BirthDate = info.BirthDate,
                Status = UserStatus.Pending
            };

            await _unitOfWork.Users.AddAsync(domainUser);
            await _unitOfWork.SaveChangesAsync();

            if (pending.VehicleInfo != null)
            {
                var v = pending.VehicleInfo;
                var vehicle = new Vehicle
                {
                    UserId = domainUser.Id,
                    Brand = v.Brand,
                    Model = v.Model,
                    Color = v.Color,
                    Year = v.Year,
                    PlateNumber = v.PlateNumber,
                    Images = new List<VehicleImage>
                    {
                        new() { ImagePath = v.FrontImagePath, Side = VehicleImageSide.Front },
                        new() { ImagePath = v.BackImagePath, Side = VehicleImageSide.Back },
                        new() { ImagePath = v.LeftImagePath, Side = VehicleImageSide.Left },
                        new() { ImagePath = v.RightImagePath, Side = VehicleImageSide.Right }
                    }
                };
                await _unitOfWork.Vehicles.AddAsync(vehicle);
                await _unitOfWork.SaveChangesAsync();
            }

            await RemovePendingAsync(dto.UserId);
            return await BuildAuthResponseAsync(appUser, domainUser);
        }

        public async Task ResendRegistrationOtpAsync(ResendRegistrationOtpDto dto)
        {
            var pending = await GetPendingAsync(dto.UserId)
                ?? throw new InvalidOperationException("Qeydiyyat sessiyasının vaxtı bitib. Zəhmət olmasa yenidən başlayın.");

            pending.OtpCode = GenerateOtp();
            pending.FailedAttempts = 0;
            await SetPendingAsync(dto.UserId, pending);

            await _emailSender.SendAsync(pending.PersonalInfo.Email, "Qeydiyyat təsdiq kodu", $"Kodunuz: {pending.OtpCode}");
        }

        // ---------- Login ----------

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var appUser = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new InvalidOperationException("Email və ya şifrə yanlışdır.");

            var passwordValid = await _userManager.CheckPasswordAsync(appUser, dto.Password);
            if (!passwordValid)
                throw new InvalidOperationException("Email və ya şifrə yanlışdır.");

            if (!appUser.EmailConfirmed)
                throw new InvalidOperationException("Email təsdiqlənməyib.");

            var domainUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.ApplicationUserId == appUser.Id)
                ?? throw new InvalidOperationException("Profil tapılmadı.");

            return await BuildAuthResponseAsync(appUser, domainUser);
        }

        // ---------- Şifrə bərpası ----------

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var appUser = await FindByContactAsync(dto.Contact, dto.Channel)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            if (dto.Channel == OtpChannel.Phone)
                await SendPhoneOtpAsync(appUser);
            else
                await SendEmailOtpAsync(appUser, "ResetPassword", "Şifrə sıfırlama kodu");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var appUser = await FindByContactAsync(dto.Contact, dto.Channel)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            var isValid = dto.Channel == OtpChannel.Phone
                ? await _userManager.VerifyChangePhoneNumberTokenAsync(appUser, dto.Code, appUser.PhoneNumber!)
                : await _userManager.VerifyUserTokenAsync(appUser, TokenOptions.DefaultEmailProvider, "ResetPassword", dto.Code);

            if (!isValid)
                throw new InvalidOperationException("Kod yanlışdır və ya vaxtı bitib.");

            await _userManager.RemovePasswordAsync(appUser);
            var result = await _userManager.AddPasswordAsync(appUser, dto.NewPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ---------- Onboarding-dən sonra ayrıca maşın əlavə etmə (artıq qeydiyyatdan keçmiş user üçün) ----------

        public async Task RegisterVehicleInfoAsync(Guid userId, RegisterVehicleDto dto)
        {
            var domainUser = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            var vehicle = new Vehicle
            {
                UserId = domainUser.Id,
                Brand = dto.Brand,
                Model = dto.Model,
                Color = dto.Color,
                Year = dto.Year,
                PlateNumber = dto.PlateNumber,
                Images = new List<VehicleImage>
                {
                    new() { ImagePath = dto.FrontImagePath, Side = VehicleImageSide.Front },
                    new() { ImagePath = dto.BackImagePath, Side = VehicleImageSide.Back },
                    new() { ImagePath = dto.LeftImagePath, Side = VehicleImageSide.Left },
                    new() { ImagePath = dto.RightImagePath, Side = VehicleImageSide.Right }
                }
            };

            await _unitOfWork.Vehicles.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        // ---------- Köməkçi metodlar ----------

        private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser appUser, User domainUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            var role = roles.FirstOrDefault() ?? nameof(RoleType.User);

            var (token, expiresAt) = _tokenService.GenerateToken(domainUser.Id, appUser.Email!, role);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = domainUser.Id,
                FullName = $"{domainUser.FirstName} {domainUser.LastName}",
                Status = domainUser.Status,
                Role = role
            };
        }

        private async Task SendPhoneOtpAsync(ApplicationUser appUser)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(appUser, appUser.PhoneNumber!);
            await _smsSender.SendAsync(appUser.PhoneNumber!, $"Share-Your-Ride təsdiq kodu: {code}");
        }

        private async Task SendEmailOtpAsync(ApplicationUser appUser, string purpose, string subject)
        {
            var code = await _userManager.GenerateUserTokenAsync(appUser, TokenOptions.DefaultEmailProvider, purpose);
            await _emailSender.SendAsync(appUser.Email!, subject, $"Kodunuz: {code}");
        }

        private async Task<ApplicationUser?> FindByContactAsync(string contact, OtpChannel channel) =>
            channel == OtpChannel.Email
                ? await _userManager.FindByEmailAsync(contact)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == contact);

        private static string MaskEmail(string email)
        {
            var atIndex = email.IndexOf('@');
            if (atIndex <= 1) return email;

            var visibleChars = Math.Min(2, atIndex);
            return $"{email[..visibleChars]}{new string('*', atIndex - visibleChars)}{email[atIndex..]}";
        }
    }
}