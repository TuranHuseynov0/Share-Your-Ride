using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Domain.Common;

namespace ShareYourRide.Domain.Entities
{
    public class TrajectoryWaypoint : BaseEntity
    {
        public Guid TrajectoryId { get; set; }
        public Trajectory Trajectory { get; set; } = default!;

        public Guid StopId { get; set; }
        public Stop Stop { get; set; } = default!;

        public int Order { get; set; }
    }
}
