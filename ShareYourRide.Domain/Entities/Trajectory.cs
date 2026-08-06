using ShareYourRide.Domain.Common;
using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ShareYourRide.Domain.Entities
{
    public class Trajectory : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public TrajectoryRole Role { get; set; }
        public DayOfWeekType Day { get; set; }
        public TimeSpan Time { get; set; }

        public Guid StartStopId { get; set; }
        public Stop StartStop { get; set; } = default!;

        public Guid EndStopId { get; set; }
        public Stop EndStop { get; set; } = default!;

        // Eyni sorğu ilə bir neçə gün üçün yaradılan trayektoriyaları qruplaşdırmaq üçün
        public Guid? ScheduleGroupId { get; set; }

        // Yalnız Driver üçün: neçə sərnişin götürə bilər
        public int? SeatCount { get; set; }

        public ICollection<TrajectoryWaypoint> Waypoints { get; set; } = new List<TrajectoryWaypoint>();

        public bool IsTemplate { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public ICollection<RideApplication> Applications { get; set; } = new List<RideApplication>();
    }
}