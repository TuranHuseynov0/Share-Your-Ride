using ShareYourRide.Application.DTOs.Template;
using ShareYourRide.Application.DTOs.Trajectory;
using ShareYourRide.Domain.Entities;
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
    public class TrajectoryService : ITrajectoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrajectoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateTrajectoryResponseDto> CreateAsync(Guid userId, CreateTrajectoryDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("İstifadəçi tapılmadı.");

            if (user.Status != UserStatus.Approved)
                throw new InvalidOperationException("Hesabınız hələ admin tərəfindən təsdiqlənməyib.");

            var trajectory = new Trajectory
            {
                UserId = userId,
                Role = dto.Role,
                Day = (DayOfWeekType)dto.DayOfWeek,
                Time = dto.Time,
                StartStopId = dto.StartStopId,
                EndStopId = dto.EndStopId,
                IsTemplate = dto.SaveAsTemplate,
            };

            await _unitOfWork.Trajectories.AddAsync(trajectory);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateTrajectoryResponseDto { Id = trajectory.Id };

            if (dto.Role == TrajectoryRole.Passenger)
                response.Matches = await FindMatchesAsync(trajectory);

            return response;
        }

        private async Task<List<DriverMatchDto>> FindMatchesAsync(Trajectory passengerTrajectory)
        {
            var driverTrajectories = await _unitOfWork.Trajectories.FindAsync(t =>
                t.Role == TrajectoryRole.Driver &&
                t.IsActive &&
                t.Day == passengerTrajectory.Day);

            var matches = new List<DriverMatchDto>();

            foreach (var dt in driverTrajectories)
            {
                var commonCount = 0;
                if (dt.StartStopId == passengerTrajectory.StartStopId || dt.StartStopId == passengerTrajectory.EndStopId)
                    commonCount++;
                if (dt.EndStopId == passengerTrajectory.StartStopId || dt.EndStopId == passengerTrajectory.EndStopId)
                    commonCount++;

                if (commonCount == 0)
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
                    CommonStopsCount = commonCount
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
                DayOfWeek = dto.Day,
                Time = dto.Time,
                StartStopId = template.StartStopId,
                EndStopId = template.EndStopId,
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
    }
}
