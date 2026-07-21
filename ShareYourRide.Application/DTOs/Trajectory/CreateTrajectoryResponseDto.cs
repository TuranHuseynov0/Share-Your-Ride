using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Trajectory
{
    public class CreateTrajectoryResponseDto
    {
        public Guid Id { get; set; }
        public List<DriverMatchDto> Matches { get; set; } = new();
    }
}
