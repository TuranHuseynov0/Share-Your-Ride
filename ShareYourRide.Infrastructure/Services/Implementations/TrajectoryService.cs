using ShareYourRide.Application.DTOs.Template;
using ShareYourRide.Application.DTOs.Trajectory;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class TrajectoryService : ITrajectoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly decimal _pricePerStop;

        // Sürücü/sərnişin saatları arasında icazə verilən tolerans
        private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(20);

        public TrajectoryService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _pricePerStop = decimal.Parse(configuration["RideSettings:PricePerStopAzn"] ?? "3.5");
        }

        private static DayOfWeekType MapDayOfWeek(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday => DayOfWeekType.Monday,
            DayOfWeek.Tuesday => DayOfWeekType.Tuesday,
            DayOfWeek.Wednesday => DayOfWeekType.Wednesday,
            DayOfWeek.Thursday => DayOfWeekType.Thursday,
            DayOfWeek.Friday => DayOfWeekType.Friday,
            DayOfWeek.Saturday => DayOfWeekType.Saturday,
            DayOfWeek.Sunday => DayOfWeekType.Sunday,
            _ => throw new InvalidOperationException("Naməlum gün.")
        };

        public async Task<CreateTrajectoryResponseDto> CreateAsync(Guid userId, CreateTrajectoryDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            if (user.Status != UserStatus.Approved)
                throw new InvalidOperationException("Hesabınız hələ admin tərəfindən təsdiqlənməyib.");

            if (dto.StartStopId == dto.EndStopId)
                throw new InvalidOperationException("Başlanğıc və son dayanacaq eyni ola bilməz.");

            if (dto.Role == TrajectoryRole.Driver && (dto.SeatCount is null || dto.SeatCount <= 0))
                throw new InvalidOperationException("Sürücü üçün oturacaq sayı seçilməlidir.");

            if (dto.SaveAsTemplate)
            {
                var existingTemplatesCount = (await _unitOfWork.Trajectories
                    .FindAsync(t => t.UserId == userId && t.IsTemplate)).Count;

                var newTemplatesCount = dto.DaysOfWeek.Distinct().Count();

                if (existingTemplatesCount + newTemplatesCount > 3)
                    throw new InvalidOperationException("Maksimum 3 şablon saxlaya bilərsiniz.");
            }

            var startStop = await _unitOfWork.Stops.GetByIdAsync(dto.StartStopId)
                ?? throw new InvalidOperationException("Başlanğıc dayanacaq tapılmadı.");
            var endStop = await _unitOfWork.Stops.GetByIdAsync(dto.EndStopId)
                ?? throw new InvalidOperationException("Son dayanacaq tapılmadı.");

            var scheduleGroupId = Guid.NewGuid();
            var response = new CreateTrajectoryResponseDto();

            foreach (var day in dto.DaysOfWeek.Distinct())
            {
                var mappedDay = MapDayOfWeek(day);

                var trajectory = new Trajectory
                {
                    UserId = userId,
                    Role = dto.Role,
                    Day = mappedDay,
                    Time = dto.Time,
                    StartStopId = dto.StartStopId,
                    EndStopId = dto.EndStopId,
                    IsTemplate = dto.SaveAsTemplate,
                    ScheduleGroupId = scheduleGroupId,
                    SeatCount = dto.Role == TrajectoryRole.Driver ? dto.SeatCount : null
                };

                await _unitOfWork.Trajectories.AddAsync(trajectory);
                await _unitOfWork.SaveChangesAsync();

                await CreateWaypointsAsync(trajectory.Id, startStop.Order, endStop.Order);

                var createdDto = new CreatedTrajectoryDto { Id = trajectory.Id, Day = mappedDay };

                if (dto.Role == TrajectoryRole.Passenger)
                    createdDto.Matches = await FindMatchesAsync(trajectory);

                response.CreatedTrajectories.Add(createdDto);
            }

            return response;
        }

        // Başlanğıc və son dayanacaq arasındakı BÜTÜN aralıq dayanacaqlarını (özləri daxil) waypoint kimi yazır.
        // Bu, matching zamanı iki trayektoriyanın "ortaq dayanacaqlarını" tapmaq üçün istifadə olunur.
        private async Task CreateWaypointsAsync(Guid trajectoryId, int startOrder, int endOrder)
        {
            var lo = Math.Min(startOrder, endOrder);
            var hi = Math.Max(startOrder, endOrder);

            var stopsInRange = (await _unitOfWork.Stops.FindAsync(s => s.Order >= lo && s.Order <= hi))
                .OrderBy(s => s.Order)
                .ToList();

            // Əgər istiqamət tərsdirsə (məs. Nəsimi -> Dərnəgül), sıranı tərsinə çeviririk
            if (startOrder > endOrder)
                stopsInRange.Reverse();

            int order = 0;
            foreach (var stop in stopsInRange)
            {
                await _unitOfWork.TrajectoryWaypoints.AddAsync(new TrajectoryWaypoint
                {
                    TrajectoryId = trajectoryId,
                    StopId = stop.Id,
                    Order = order++
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<List<DriverMatchDto>> FindMatchesAsync(Trajectory passengerTrajectory)
        {
            var passengerStopIds = (await _unitOfWork.TrajectoryWaypoints
                    .FindAsync(w => w.TrajectoryId == passengerTrajectory.Id))
                .Select(w => w.StopId)
                .ToHashSet();

            var driverTrajectories = await _unitOfWork.Trajectories.FindAsync(t =>
                t.Role == TrajectoryRole.Driver &&
                t.IsActive &&
                !t.IsTemplate &&
                t.Day == passengerTrajectory.Day);

            var matches = new List<DriverMatchDto>();

            foreach (var dt in driverTrajectories)
            {
                if ((dt.Time - passengerTrajectory.Time).Duration() > TimeWindow)
                    continue;

                var driverStopIds = (await _unitOfWork.TrajectoryWaypoints
                        .FindAsync(w => w.TrajectoryId == dt.Id))
                    .Select(w => w.StopId)
                    .ToHashSet();

                var commonCount = passengerStopIds.Intersect(driverStopIds).Count();
                if (commonCount == 0)
                    continue;

                var approvedCount = (await _unitOfWork.RideApplications.FindAsync(a =>
                    a.DriverTrajectoryId == dt.Id && a.Status == RideApplicationStatus.Approved)).Count;

                var remainingSeats = (dt.SeatCount ?? 0) - approvedCount;
                if (remainingSeats <= 0)
                    continue;

                var driverUser = await _unitOfWork.Users.GetByIdAsync(dt.UserId);
                var vehicle = (await _unitOfWork.Vehicles.FindAsync(v => v.UserId == dt.UserId)).FirstOrDefault();

                if (driverUser == null || vehicle == null)
                    continue;

                matches.Add(new DriverMatchDto
                {
                    DriverTrajectoryId = dt.Id,
                    DriverFullName = $"{driverUser.FirstName} {driverUser.LastName}",
                    VehicleBrand = vehicle.Brand,
                    VehicleModel = vehicle.Model,
                    VehicleColor = vehicle.Color,
                    DriverTime = dt.Time,
                    CommonStopsCount = commonCount,
                    Price = commonCount * _pricePerStop,
                    RemainingSeats = remainingSeats
                });
            }

            return matches
                .OrderByDescending(m => m.CommonStopsCount)
                .ThenBy(m => Math.Abs((m.DriverTime - passengerTrajectory.Time).TotalMinutes))
                .ToList();
        }

        public async Task<CreateTrajectoryResponseDto> CreateFromTemplateAsync(Guid userId, CreateFromTemplateDto dto)
        {
            var template = await _unitOfWork.Trajectories.GetByIdAsync(dto.TemplateTrajectoryId)
                ?? throw new InvalidOperationException("Şablon tapılmadı.");

            if (template.UserId != userId || !template.IsTemplate)
                throw new InvalidOperationException("Bu şablon sizə aid deyil.");

            return await CreateAsync(userId, new CreateTrajectoryDto
            {
                Role = template.Role,
                DaysOfWeek = new List<DayOfWeek> { dto.Day },
                Time = dto.Time,
                StartStopId = template.StartStopId,
                EndStopId = template.EndStopId,
                SeatCount = template.SeatCount,
                SaveAsTemplate = false
            });
        }

        public async Task<IReadOnlyList<TemplateDto>> GetMyTemplatesAsync(Guid userId)
        {
            var templates = await _unitOfWork.Trajectories.FindAsync(t => t.UserId == userId && t.IsTemplate);
            var result = new List<TemplateDto>();

            foreach (var t in templates)
            {
                var startStop = await _unitOfWork.Stops.GetByIdAsync(t.StartStopId);
                var endStop = await _unitOfWork.Stops.GetByIdAsync(t.EndStopId);

                result.Add(new TemplateDto
                {
                    Id = t.Id,
                    StartStopName = startStop?.Name ?? "N/A",
                    EndStopName = endStop?.Name ?? "N/A"
                });
            }

            return result;
        }

        public async Task<IReadOnlyList<TrajectoryDto>> GetMyTrajectoriesAsync(Guid userId)
        {
            var trajectories = await _unitOfWork.Trajectories.FindAsync(t => t.UserId == userId && !t.IsTemplate);
            var result = new List<TrajectoryDto>();

            foreach (var t in trajectories)
            {
                var startStop = await _unitOfWork.Stops.GetByIdAsync(t.StartStopId);
                var endStop = await _unitOfWork.Stops.GetByIdAsync(t.EndStopId);

                result.Add(new TrajectoryDto
                {
                    Id = t.Id,
                    Role = t.Role,
                    Day = t.Day,
                    Time = t.Time,
                    StartStopName = startStop?.Name ?? "N/A",
                    EndStopName = endStop?.Name ?? "N/A",
                    IsTemplate = t.IsTemplate,
                    IsActive = t.IsActive
                });
            }

            return result.OrderBy(t => t.Day).ThenBy(t => t.Time).ToList();
        }

        public async Task DeleteTemplateAsync(Guid userId, Guid templateId)
        {
            var template = await _unitOfWork.Trajectories.GetByIdAsync(templateId)
                ?? throw new InvalidOperationException("Şablon tapılmadı.");

            if (template.UserId != userId || !template.IsTemplate)
                throw new InvalidOperationException("Bu şablon sizə aid deyil.");

            _unitOfWork.Trajectories.Remove(template);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}