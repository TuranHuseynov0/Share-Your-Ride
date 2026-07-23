using ShareYourRide.Application.DTOs.RideApplication;
using ShareYourRide.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IRideApplicationService
    {
        Task<RideApplicationDto> ApplyAsync(Guid passenger, CreateRideApplicationDto dto);
        Task AcceptAsync(Guid passenger, Guid applicationId);
        Task RejectAsync(Guid passenger, Guid applicationId);
        Task<IReadOnlyList<RideApplicationDto>> GetMyApplicationsAsync(Guid passengerUserId);
        Task<IReadOnlyList<RideApplicationDto>> GetIncomingApplicationsAsync(Guid driverUserId);
    }
}
