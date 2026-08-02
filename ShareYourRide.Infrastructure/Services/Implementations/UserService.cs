using Microsoft.AspNetCore.Identity;
using ShareYourRide.Application.DTOs.User;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Identity;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            var appUser = await _userManager.FindByIdAsync(user.ApplicationUserId.ToString())
                ?? throw new InvalidOperationException("Hesab tapılmadı.");

            var hasVehicle = (await _unitOfWork.Vehicles.FindAsync(v => v.UserId == user.Id)).Any();

            var applications = await _unitOfWork.RideApplications.FindAsync(a => a.PassengerUserId == user.Id);
            var total = applications.Count;
            var completed = applications.Count(a => a.Status == RideApplicationStatus.Approved);
            var rejected = applications.Count(a => a.Status == RideApplicationStatus.Rejected);
            var cancellationRate = total == 0 ? 0 : Math.Round((decimal)rejected / total * 100, 0);

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = appUser.Email ?? string.Empty,
                PhoneNumber = appUser.PhoneNumber ?? string.Empty,
                MaskedFinCode = MaskFinCode(user.FinCode),
                ProfileImagePath = user.ProfileImagePath,
                Bio = user.Bio,
                Status = user.Status,
                HasVehicle = hasVehicle,
                Rating = user.Rating,
                MemberSinceYear = user.CreatedAt.Year,
                CompletedRideCount = completed,
                CancellationRatePercent = cancellationRate
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            var appUser = await _userManager.FindByIdAsync(user.ApplicationUserId.ToString())
                ?? throw new InvalidOperationException("Hesab tapılmadı.");

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (dto.Bio != null)
                user.Bio = dto.Bio;

            _unitOfWork.Users.Update(user);

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && appUser.PhoneNumber != dto.PhoneNumber)
            {
                var phoneExists = _userManager.Users.Any(u => u.PhoneNumber == dto.PhoneNumber && u.Id != appUser.Id);
                if (phoneExists)
                    throw new InvalidOperationException("Bu telefon nömrəsi artıq istifadə olunur.");

                appUser.PhoneNumber = dto.PhoneNumber;
                appUser.PhoneNumberConfirmed = false;
                await _userManager.UpdateAsync(appUser);
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetProfileAsync(userId);
        }

        private static string MaskFinCode(string finCode)
        {
            if (finCode.Length <= 2) return finCode;
            return $"{finCode[..2]}{new string('*', finCode.Length - 2)}";
        }
    }
}