using ShareYourRide.Domain.Common;
using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Entities
{
    public class RideApplication : BaseEntity
    {
        public Guid DriverTrajectoryId { get; set; }
        public Trajectory DriverTrajectory { get; set; } = default!;

        public Guid PassengerUserId { get; set; }
        public User PassengerUser { get; set; } = default!;

        public RideApplicationStatus Status { get; set; } = RideApplicationStatus.Pending;
    }
}
