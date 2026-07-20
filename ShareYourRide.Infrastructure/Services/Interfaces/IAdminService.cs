using ShareYourRide.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IReadOnlyList<PendingUserListItemDto>> GetPendingUsersAsync();
        Task ApproveUserAsync(ApproveUserDto dto);
        Task RejectUserAsync(RejectUserDto dto);
    }
}
