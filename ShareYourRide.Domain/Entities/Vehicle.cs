using ShareYourRide.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public string Brand { get; set; } = default!;
        public string Model { get; set; } = default!;
        public string Color { get; set; } = default!;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = default!;

        public ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();
    }
}
