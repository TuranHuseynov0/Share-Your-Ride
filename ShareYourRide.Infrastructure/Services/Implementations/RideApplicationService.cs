using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ShareYourRide.Application.DTOs.MatchedRide;
using ShareYourRide.Application.DTOs.RideApplication;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Identity;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class RideApplicationService : IRideApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly decimal _pricePerStop;
        private readonly UserManager<ApplicationUser> _userManager;

        public RideApplicationService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _pricePerStop = decimal.Parse(configuration["RideSettings:PricePerStopAzn"] ?? "3.5");
            _userManager = userManager;
        }

        public async Task<RideApplicationDto> ApplyAsync(Guid passengerUserId, CreateRideApplicationDto dto)
        {
            var passenger = await _unitOfWork.Users.GetByIdAsync(passengerUserId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            if (passenger.Status != UserStatus.Approved)
                throw new InvalidOperationException("Hesabınız hələ admin tərəfindən təsdiqlənməyib.");

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(dto.DriverTrajectoryId)
                ?? throw new InvalidOperationException("Marşrut tapılmadı.");

            if (driverTrajectory.Role != TrajectoryRole.Driver || !driverTrajectory.IsActive)
                throw new InvalidOperationException("Bu marşrut sürücü elanı deyil.");

            var passengerTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(dto.PassengerTrajectoryId)
                ?? throw new InvalidOperationException("Sərnişin trayektoriyası tapılmadı.");

            if (passengerTrajectory.UserId != passengerUserId || passengerTrajectory.Role != TrajectoryRole.Passenger)
                throw new InvalidOperationException("Bu trayektoriya sizə aid deyil.");

            if (passengerTrajectory.Day != driverTrajectory.Day)
                throw new InvalidOperationException("Sürücü ilə sərnişinin günü üst-üstə düşmür.");

            var alreadyApplied = (await _unitOfWork.RideApplications.FindAsync(a =>
                a.DriverTrajectoryId == dto.DriverTrajectoryId &&
                a.PassengerUserId == passengerUserId)).Any();

            if (alreadyApplied)
                throw new InvalidOperationException("Bu marşruta artıq müraciət etmisiniz.");

            var approvedCount = (await _unitOfWork.RideApplications.FindAsync(a =>
                a.DriverTrajectoryId == dto.DriverTrajectoryId && a.Status == RideApplicationStatus.Approved)).Count;

            if (approvedCount >= (driverTrajectory.SeatCount ?? 0))
                throw new InvalidOperationException("Bu marşrutda boş yer qalmayıb.");

            var commonStopsCount = await GetCommonStopsCountAsync(driverTrajectory.Id, passengerTrajectory.Id);
            if (commonStopsCount == 0)
                throw new InvalidOperationException("Marşrutlar arasında ortaq dayanacaq tapılmadı.");

            var application = new RideApplication
            {
                DriverTrajectoryId = dto.DriverTrajectoryId,
                PassengerTrajectoryId = dto.PassengerTrajectoryId,
                PassengerUserId = passengerUserId,
                Status = RideApplicationStatus.Pending,
                CommonStopsCount = commonStopsCount,
                Price = commonStopsCount * _pricePerStop
            };

            await _unitOfWork.RideApplications.AddAsync(application);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(application);
        }

        private async Task<int> GetCommonStopsCountAsync(Guid driverTrajectoryId, Guid passengerTrajectoryId)
        {
            var driverStopIds = (await _unitOfWork.TrajectoryWaypoints
                    .FindAsync(w => w.TrajectoryId == driverTrajectoryId))
                .Select(w => w.StopId)
                .ToHashSet();

            var passengerStopIds = (await _unitOfWork.TrajectoryWaypoints
                    .FindAsync(w => w.TrajectoryId == passengerTrajectoryId))
                .Select(w => w.StopId)
                .ToHashSet();

            return driverStopIds.Intersect(passengerStopIds).Count();
        }

        public async Task AcceptAsync(Guid driverUserId, Guid applicationId)
        {
            var application = await GetOwnedApplicationAsync(driverUserId, applicationId);

            if (application.Status != RideApplicationStatus.Pending)
                throw new InvalidOperationException("Bu müraciət artıq cavablandırılıb.");

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(application.DriverTrajectoryId)
                ?? throw new InvalidOperationException("Marşrut tapılmadı.");

            var approvedCount = (await _unitOfWork.RideApplications.FindAsync(a =>
                a.DriverTrajectoryId == application.DriverTrajectoryId && a.Status == RideApplicationStatus.Approved)).Count;

            if (approvedCount >= (driverTrajectory.SeatCount ?? 0))
                throw new InvalidOperationException("Bu marşrutda boş yer qalmayıb.");

            var passengerWallet = await _unitOfWork.Wallets.SingleOrDefaultAsync(w => w.UserId == application.PassengerUserId)
                ?? throw new InvalidOperationException("Sərnişinin balans hesabı tapılmadı.");

            var driverWallet = await _unitOfWork.Wallets.SingleOrDefaultAsync(w => w.UserId == driverUserId)
                ?? throw new InvalidOperationException("Sürücünün balans hesabı tapılmadı.");

            if (passengerWallet.Balance < application.Price)
                throw new InvalidOperationException("Sərnişinin balansı kifayət deyil.");

            passengerWallet.Balance -= application.Price;
            driverWallet.Balance += application.Price;

            _unitOfWork.Wallets.Update(passengerWallet);
            _unitOfWork.Wallets.Update(driverWallet);

            await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
            {
                WalletId = passengerWallet.Id,
                Amount = application.Price,
                Type = TransactionType.RidePayment,
                RelatedRideApplicationId = application.Id
            });

            await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
            {
                WalletId = driverWallet.Id,
                Amount = application.Price,
                Type = TransactionType.RideEarning,
                RelatedRideApplicationId = application.Id
            });

            application.Status = RideApplicationStatus.Approved;
            _unitOfWork.RideApplications.Update(application);

            await _unitOfWork.SaveChangesAsync();

            // NOT: Chat thread yaradılması 4-cü tapşırıqda buraya əlavə olunacaq
            // (ChatService inteqrasiyası zamanı).
        }

        public async Task RejectAsync(Guid driverUserId, Guid applicationId)
        {
            var application = await GetOwnedApplicationAsync(driverUserId, applicationId);

            if (application.Status != RideApplicationStatus.Pending)
                throw new InvalidOperationException("Bu müraciət artıq cavablandırılıb.");

            application.Status = RideApplicationStatus.Rejected;
            _unitOfWork.RideApplications.Update(application);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<RideApplicationDto>> GetMyApplicationsAsync(Guid passengerUserId)
        {
            var applications = await _unitOfWork.RideApplications.FindAsync(a => a.PassengerUserId == passengerUserId);
            var result = new List<RideApplicationDto>();
            foreach (var a in applications.OrderByDescending(a => a.CreatedAt))
                result.Add(await MapToDtoAsync(a));
            return result;
        }

        // Sürücünün "gələn bildirişlər" siyahısı — sərnişinin məlumatları və ortaq dayanacaqlar burada gəlir
        public async Task<IReadOnlyList<RideApplicationDto>> GetIncomingApplicationsAsync(Guid driverUserId)
        {
            var driverTrajectories = await _unitOfWork.Trajectories.FindAsync(t =>
                t.UserId == driverUserId && t.Role == TrajectoryRole.Driver);
            var driverTrajectoryIds = driverTrajectories.Select(t => t.Id).ToHashSet();

            var applications = await _unitOfWork.RideApplications.FindAsync(a =>
                driverTrajectoryIds.Contains(a.DriverTrajectoryId));

            var result = new List<RideApplicationDto>();
            foreach (var a in applications.OrderByDescending(a => a.CreatedAt))
                result.Add(await MapToDtoAsync(a));
            return result;
        }

        public async Task<IReadOnlyList<RideApplicationDto>> GetMyCompletedRidesAsync(Guid passengerUserId)
        {
            var applications = await _unitOfWork.RideApplications.FindAsync(a =>
                a.PassengerUserId == passengerUserId &&
                (a.Status == RideApplicationStatus.Approved || a.Status == RideApplicationStatus.Completed));

            var result = new List<RideApplicationDto>();
            foreach (var a in applications.OrderByDescending(a => a.CreatedAt))
                result.Add(await MapToDtoAsync(a));
            return result;
        }

        public async Task<MatchedRideDto?> GetCurrentMatchAsync(Guid passengerUserId)
        {
            var applications = await _unitOfWork.RideApplications.FindAsync(a =>
                a.PassengerUserId == passengerUserId && a.Status == RideApplicationStatus.Approved);

            var latest = applications.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
            if (latest == null)
                return null;

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(latest.DriverTrajectoryId);
            if (driverTrajectory == null)
                return null;

            var driverUser = await _unitOfWork.Users.GetByIdAsync(driverTrajectory.UserId);
            if (driverUser == null)
                return null;

            var driverAppUser = await _userManager.FindByIdAsync(driverUser.ApplicationUserId.ToString());

            var vehicle = (await _unitOfWork.Vehicles.FindAsync(v => v.UserId == driverUser.Id)).FirstOrDefault();
            if (vehicle == null)
                return null;

            return new MatchedRideDto
            {
                DriverFullName = $"{driverUser.FirstName} {driverUser.LastName}",
                DriverPhoneNumber = driverAppUser?.PhoneNumber ?? string.Empty,
                VehicleBrand = vehicle.Brand,
                VehicleModel = vehicle.Model,
                VehicleColor = vehicle.Color,
                PlateNumber = vehicle.PlateNumber,
                Fare = latest.Price,
                MatchedAt = latest.CreatedAt
            };
        }

        private async Task<RideApplication> GetOwnedApplicationAsync(Guid driverUserId, Guid applicationId)
        {
            var application = await _unitOfWork.RideApplications.GetByIdAsync(applicationId)
                ?? throw new InvalidOperationException("Müraciət tapılmadı.");

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(application.DriverTrajectoryId)
                ?? throw new InvalidOperationException("Marşrut tapılmadı.");

            if (driverTrajectory.UserId != driverUserId)
                throw new InvalidOperationException("Bu müraciətə icazəniz yoxdur.");

            return application;
        }

        private async Task<RideApplicationDto> MapToDtoAsync(RideApplication application)
        {
            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(application.DriverTrajectoryId);
            var passenger = await _unitOfWork.Users.GetByIdAsync(application.PassengerUserId);
            var driver = driverTrajectory != null ? await _unitOfWork.Users.GetByIdAsync(driverTrajectory.UserId) : null;

            var startStop = driverTrajectory != null ? await _unitOfWork.Stops.GetByIdAsync(driverTrajectory.StartStopId) : null;
            var endStop = driverTrajectory != null ? await _unitOfWork.Stops.GetByIdAsync(driverTrajectory.EndStopId) : null;

            return new RideApplicationDto
            {
                Id = application.Id,
                DriverTrajectoryId = application.DriverTrajectoryId,
                PassengerFullName = passenger != null ? $"{passenger.FirstName} {passenger.LastName}" : "N/A",
                DriverFullName = driver != null ? $"{driver.FirstName} {driver.LastName}" : "N/A",
                Status = application.Status,
                CreatedAt = application.CreatedAt,
                Price = application.Price,
                CommonStopsCount = application.CommonStopsCount,
                StartStopName = startStop?.Name ?? "N/A",
                EndStopName = endStop?.Name ?? "N/A"
            };
        }
    }
}