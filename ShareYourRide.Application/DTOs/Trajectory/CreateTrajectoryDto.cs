using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class CreateTrajectoryDto
    {
        [Required] public TrajectoryRole Role { get; set; }

        // Bir neçə gün seçilə bilər (B.e, Ç.a, B kimi ayrı-ayrı)
        [Required, MinLength(1)] public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        [Required] public TimeSpan Time { get; set; }
        [Required] public Guid StartStopId { get; set; }
        [Required] public Guid EndStopId { get; set; }

        // Yalnız Role == Driver olduqda tələb olunur
        public int? SeatCount { get; set; }

        public bool SaveAsTemplate { get; set; } = false;
    }
}