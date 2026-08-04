using ShareYourRide.Domain.Common;
using System;

namespace ShareYourRide.Domain.Entities
{
    public class VehicleModel : BaseEntity
    {
        public Guid VehicleBrandId { get; set; }
        public VehicleBrand VehicleBrand { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}