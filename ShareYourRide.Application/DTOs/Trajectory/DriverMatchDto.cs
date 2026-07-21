using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class DriverMatchDto
    {
        public Guid DriverTrajectoryId { get; set; }
        public string DriverFullName { get; set; } = default!;
        public string VehicleBrand { get; set; } = default!;
        public string VehicleModel { get; set; } = default!;
        public string VehicleColor { get; set; } = default!;
        public TimeSpan DriverTime { get; set; }
        public int CommonStopsCount { get; set; }
    }
}
