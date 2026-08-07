using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class CreateTrajectoryResponseDto
    {
        public List<CreatedTrajectoryDto> CreatedTrajectories { get; set; } = new();
    }

    public class CreatedTrajectoryDto
    {
        public Guid Id { get; set; }
        public DayOfWeekType Day { get; set; }

        // Yalnız Role == Passenger olan trayektoriyalar üçün doldurulur
        public List<DriverMatchDto> Matches { get; set; } = new();
    }
}