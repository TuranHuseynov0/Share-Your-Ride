using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Domain.Enums;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class TrajectoryDto
    {
        public Guid Id { get; set; }
        public TrajectoryRole Role { get; set; }
        public DayOfWeekType Day { get; set; }
        public TimeSpan Time { get; set; }
        public string StartStopName { get; set; } = default!;
        public string EndStopName { get; set; } = default!;
        public bool IsTemplate { get; set; }
        public bool IsActive { get; set; }
    }
}
