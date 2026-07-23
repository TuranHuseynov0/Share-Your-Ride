using ShareYourRide.Application.DTOs.RideApplication;
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
    public class RideApplicationService : IRideApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RideApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RideApplicationDto> ApplyAsync(Guid passengerUserId, CreateRideApplicationDto dto)
        {
            var passenger = await _unitOfWork.Users.GetByIdAsync(passengerUserId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            if (passenger.Status != UserStatus.Approved)
                throw new InvalidOperationException("Hesabınız hələ admin tərəfindən təsdiqlənməyib.");

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(dto.DriverTrajectoryId)
                ?? throw new InvalidOperationException("Marşrut tapılmadı.");

            if (driverTrajectory.Role != Domain.Enums.TrajectoryRole.Driver || !driverTrajectory.IsActive)
                throw new InvalidOperationException("Bu marşrut sürücü elanı deyil.");

            var alreadyApplied = (await _unitOfWork.RideApplications.FindAsync(a =>
                a.DriverTrajectoryId == dto.DriverTrajectoryId && a.PassengerUserId == passengerUserId)).Any();

            if (alreadyApplied)
                throw new InvalidOperationException("Bu marşruta artıq müraciət etmisiniz.");

            var application = new Domain.Entities.RideApplication
            {
                DriverTrajectoryId = dto.DriverTrajectoryId,
                PassengerUserId = passengerUserId,
                Status = RideApplicationStatus.Pending
            };

            await _unitOfWork.RideApplications.AddAsync(application);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(application);
        }

        public async Task AcceptAsync(Guid driverUserId, Guid applicationId)
        {
            var application = await GetOwnedApplicationAsync(driverUserId, applicationId);
            application.Status = RideApplicationStatus.Approved;
            _unitOfWork.RideApplications.Update(application);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectAsync(Guid driverUserId, Guid applicationId)
        {
            var application = await GetOwnedApplicationAsync(driverUserId, applicationId);
            application.Status = RideApplicationStatus.Rejected;
            _unitOfWork.RideApplications.Update(application);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<RideApplicationDto>> GetMyApplicationsAsync(Guid passengerUserId)
        {
            var applications = await _unitOfWork.RideApplications.FindAsync(a => a.PassengerUserId == passengerUserId);
            var result = new List<RideApplicationDto>();
            foreach (var a in applications)
                result.Add(await MapToDtoAsync(a));
            return result;
        }

        public async Task<IReadOnlyList<RideApplicationDto>> GetIncomingApplicationsAsync(Guid driverUserId)
        {
            var driverTrajectories = await _unitOfWork.Trajectories.FindAsync(t => t.UserId == driverUserId && t.Role == Domain.Enums.TrajectoryRole.Driver);
            var driverTrajectoryIds = driverTrajectories.Select(t => t.Id).ToList();

            var applications = await _unitOfWork.RideApplications.FindAsync(a => driverTrajectoryIds.Contains(a.DriverTrajectoryId));
            var result = new List<RideApplicationDto>();
            foreach (var a in applications)
                result.Add(await MapToDtoAsync(a));
            return result;
        }

        private async Task<Domain.Entities.RideApplication> GetOwnedApplicationAsync(Guid driverUserId, Guid applicationId)
        {
            var application = await _unitOfWork.RideApplications.GetByIdAsync(applicationId)
                ?? throw new InvalidOperationException("Müraciət tapılmadı.");

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(application.DriverTrajectoryId)
                ?? throw new InvalidOperationException("Marşrut tapılmadı.");

            if (driverTrajectory.UserId != driverUserId)
                throw new InvalidOperationException("Bu müraciətə icazəniz yoxdur.");

            return application;
        }

        private async Task<RideApplicationDto> MapToDtoAsync(Domain.Entities.RideApplication application)
        {
            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(application.DriverTrajectoryId);
            var passenger = await _unitOfWork.Users.GetByIdAsync(application.PassengerUserId);
            var driver = driverTrajectory != null ? await _unitOfWork.Users.GetByIdAsync(driverTrajectory.UserId) : null;

            return new RideApplicationDto
            {
                Id = application.Id,
                DriverTrajectoryId = application.DriverTrajectoryId,
                PassengerFullName = passenger != null ? $"{passenger.FirstName} {passenger.LastName}" : "N/A",
                DriverFullName = driver != null ? $"{driver.FirstName} {driver.LastName}" : "N/A",
                Status = application.Status,
                CreatedAt = application.CreatedAt
            };
        }
    }
}
