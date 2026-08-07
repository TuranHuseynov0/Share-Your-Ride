using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class CreateFromTemplateDto
    {
        [Required] public Guid TemplateTrajectoryId { get; set; }
        [Required] public DayOfWeek Day { get; set; }
        [Required] public TimeSpan Time { get; set; }
    }
}
