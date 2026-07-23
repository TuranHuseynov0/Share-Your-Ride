using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.RideApplication
{
    public class CreateRideApplicationDto
    {
        [Required] public Guid DriverTrajectoryId { get; set; }
    }
}
