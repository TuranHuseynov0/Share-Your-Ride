using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class CreateTrajectoryDto
    {
        [Required] public TrajectoryRole Role { get; set; }
        [Required] public DayOfWeek DayOfWeek { get; set; }
        [Required] public TimeSpan Time { get; set; }
        [Required] public Guid StartStopId { get; set; }
        [Required] public Guid EndStopId { get; set; }
    }
}
