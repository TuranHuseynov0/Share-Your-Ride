using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Application.DTOs.Trajectory;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrajectoryController : ControllerBase
    {
        private readonly ITrajectoryService _trajectoryService;

        public TrajectoryController(ITrajectoryService trajectoryService)
        {
            _trajectoryService = trajectoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrajectoryDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var result = await _trajectoryService.CreateAsync(userId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
