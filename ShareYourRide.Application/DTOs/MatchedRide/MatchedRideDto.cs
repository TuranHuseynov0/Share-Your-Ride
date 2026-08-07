using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.MatchedRide
{
    public class MatchedRideDto
    {
        public string DriverFullName { get; set; } = default!;
        public string DriverPhoneNumber { get; set; } = default!;
        public string VehicleBrand { get; set; } = default!;
        public string VehicleModel { get; set; } = default!;
        public string VehicleColor { get; set; } = default!;
        public string PlateNumber { get; set; } = default!;
        public decimal Fare { get; set; }
        public DateTime MatchedAt { get; set; }
    }
}
