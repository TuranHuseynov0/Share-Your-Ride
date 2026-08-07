using ShareYourRide.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShareYourRide.Application.DTOs.RideApplication
{
    public class RideApplicationDto
    {
        [Required] public Guid Id { get; set; }
        [Required] public Guid DriverTrajectoryId { get; set; }
        [Required] public string PassengerFullName { get; set; } = default!;
        [Required] public string DriverFullName { get; set; } = default!;
        public RideApplicationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
        public int CommonStopsCount { get; set; }
        public string StartStopName { get; set; } = default!;
        public string EndStopName { get; set; } = default!;
    }
}
