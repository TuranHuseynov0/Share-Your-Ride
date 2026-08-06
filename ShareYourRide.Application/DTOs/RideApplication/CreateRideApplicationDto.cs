using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;

namespace ShareYourRide.Application.DTOs.RideApplication
{
    public class CreateRideApplicationDto
    {
        [Required] public Guid DriverTrajectoryId { get; set; }

        // Sərnişinin hansı öz trayektoriyası ilə bu sürücüyə müraciət etdiyi
        [Required] public Guid PassengerTrajectoryId { get; set; }
    }
}
