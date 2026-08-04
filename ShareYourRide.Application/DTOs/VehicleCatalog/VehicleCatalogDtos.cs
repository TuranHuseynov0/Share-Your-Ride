using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.VehicleCatalog
{
    public class VehicleBrandDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class VehicleModelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
    }

    public class VehicleColorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? HexCode { get; set; }
    }
}