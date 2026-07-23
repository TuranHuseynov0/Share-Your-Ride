using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
