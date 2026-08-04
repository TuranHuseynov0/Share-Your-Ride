using ShareYourRide.Domain.Common;
using System.Collections.Generic;

namespace ShareYourRide.Domain.Entities
{
    public class VehicleBrand : BaseEntity
    {
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
    }
}