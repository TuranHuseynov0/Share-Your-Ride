using ShareYourRide.Domain.Common;
using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Entities
{
    public class VehicleImage : BaseEntity
    {
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = default!;
        public string ImagePath { get; set; } = default!;
        public VehicleImageSide Side { get; set; }
    }
}
