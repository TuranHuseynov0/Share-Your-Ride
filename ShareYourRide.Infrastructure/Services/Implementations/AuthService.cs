using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShareYourRide.Application.DTOs.Auth;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Identity;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            ISmsSender smsSender,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _smsSender = smsSender;
            _emailSender = emailSender;
        }

        public async Task<RegisterPersonalInfoResponseDto> RegisterPersonalInfoAsync(RegisterPersonalInfoDto dto)
        {
            var finExists = (await _unitOfWork.Users.FindAsync(u => u.FinCode == dto.FinCode)).Any();
            if (finExists)
                throw new InvalidOperationException("Bu FIN kod artıq qeydiyyatdan keçib.");

            var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (phoneExists)
                throw new InvalidOperationException("Bu telefon nömrəsi artıq qeydiyyatdan keçib.");

            var appUser = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var identityResult = await _userManager.CreateAsync(appUser, dto.Password);
            if (!identityResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", identityResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(appUser, nameof(RoleType.User));

            var domainUser = new User
            {
                ApplicationUserId = appUser.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FinCode = dto.FinCode,
                BirthDate = dto.BirthDate,
                Status = UserStatus.Pending
            };

            await _unitOfWork.Users.AddAsync(domainUser);
            await _unitOfWork.SaveChangesAsync();

            await SendPhoneOtpAsync(appUser);

            return new RegisterPersonalInfoResponseDto
            {
                UserId = domainUser.Id,
                MaskedPhoneNumber = MaskPhone(dto.PhoneNumber),
                OtpExpirySeconds = 60,
                RequiresVehicleInfo = dto.Role == TrajectoryRole.Driver
            };
        }

        public async Task RegisterVehicleInfoAsync(RegisterVehicleDto dto)
        {
            var domainUser = await _unitOfWork.Users.GetByIdAsync(dto.UserId)
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

        public async Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto)
        {
            var domainUser = await _unitOfWork.Users.GetByIdAsync(dto.UserId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            var appUser = await _userManager.FindByIdAsync(domainUser.ApplicationUserId.ToString())
                ?? throw new InvalidOperationException("Hesab tapılmadı.");

            var isValid = await _userManager.VerifyChangePhoneNumberTokenAsync(appUser, dto.Code, appUser.PhoneNumber!);
            if (!isValid)
                throw new InvalidOperationException("Kod yanlışdır və ya vaxtı bitib.");

            appUser.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(appUser);

            return await BuildAuthResponseAsync(appUser, domainUser);
        }

        public async Task ResendRegistrationOtpAsync(ResendRegistrationOtpDto dto)
        {
            var domainUser = await _unitOfWork.Users.GetByIdAsync(dto.UserId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            var appUser = await _userManager.FindByIdAsync(domainUser.ApplicationUserId.ToString())
                ?? throw new InvalidOperationException("Hesab tapılmadı.");

            await SendPhoneOtpAsync(appUser);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var appUser = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new InvalidOperationException("Email və ya şifrə yanlışdır.");

            var passwordValid = await _userManager.CheckPasswordAsync(appUser, dto.Password);
            if (!passwordValid)
                throw new InvalidOperationException("Email və ya şifrə yanlışdır.");

            if (!appUser.PhoneNumberConfirmed)
                throw new InvalidOperationException("Telefon nömrəsi təsdiqlənməyib.");

            var domainUser = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.ApplicationUserId == appUser.Id)
                ?? throw new InvalidOperationException("Profil tapılmadı.");

            return await BuildAuthResponseAsync(appUser, domainUser);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var appUser = await FindByContactAsync(dto.Contact, dto.Channel)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            if (dto.Channel == OtpChannel.Phone)
                await SendPhoneOtpAsync(appUser);
            else
                await SendEmailOtpAsync(appUser);
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

        // ---- daxili köməkçi metodlar ----

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
                Status = domainUser.Status
            };
        }

        private async Task SendPhoneOtpAsync(ApplicationUser appUser)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(appUser, appUser.PhoneNumber!);
            await _smsSender.SendAsync(appUser.PhoneNumber!, $"Share-Your-Ride təsdiq kodu: {code}");
        }

        private async Task SendEmailOtpAsync(ApplicationUser appUser)
        {
            var code = await _userManager.GenerateUserTokenAsync(appUser, TokenOptions.DefaultEmailProvider, "ResetPassword");
            await _emailSender.SendAsync(appUser.Email!, "Şifrə sıfırlama kodu", $"Kodunuz: {code}");
        }

        private async Task<ApplicationUser?> FindByContactAsync(string contact, OtpChannel channel) =>
            channel == OtpChannel.Email
                ? await _userManager.FindByEmailAsync(contact)
                : await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == contact);

        private static string MaskPhone(string phone) =>
            phone.Length <= 4 ? phone : $"{phone[..^4].Replace(phone[4..^4], new string('*', phone.Length - 8))}{phone[^4..]}";
    }
}