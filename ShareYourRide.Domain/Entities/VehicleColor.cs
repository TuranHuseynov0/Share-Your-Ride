using ShareYourRide.Domain.Common;

namespace ShareYourRide.Domain.Entities
{
    public class VehicleColor : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? HexCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}