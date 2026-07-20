using ShareYourRide.Application.DTOs.Admin;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private static readonly string[] AllowedRejectReasons =
        {
            "Şəkil aydın deyil",
            "FIN kod uyğun gəlmir",
            "Sənəd etibarsızdır"
        };

        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<PendingUserListItemDto>> GetPendingUsersAsync()
        {
            var pendingUsers = await _unitOfWork.Users.FindAsync(u => u.Status == UserStatus.Pending);

            var result = new List<PendingUserListItemDto>();
            foreach (var user in pendingUsers)
            {
                var hasVehicle = (await _unitOfWork.Vehicles.FindAsync(v => v.UserId == user.Id)).Any();

                result.Add(new PendingUserListItemDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    FinCode = user.FinCode,
                    HasVehicle = hasVehicle,
                    CreatedAt = user.CreatedAt
                });
            }

            return result;
        }

        public async Task ApproveUserAsync(ApproveUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            user.Status = UserStatus.Approved;
            user.RejectReason = null;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectUserAsync(RejectUserDto dto)
        {
            if (!AllowedRejectReasons.Contains(dto.Reason))
                throw new InvalidOperationException("Rədd səbəbi siyahıdan seçilməlidir.");

            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            user.Status = UserStatus.Rejected;
            user.RejectReason = dto.Reason;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
