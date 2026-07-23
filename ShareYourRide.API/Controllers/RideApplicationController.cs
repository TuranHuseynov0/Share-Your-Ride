using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Application.DTOs.RideApplication;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RideApplicationController : ControllerBase
    {
        private readonly IRideApplicationService _rideApplicationService;

        public RideApplicationController(IRideApplicationService rideApplicationService)
        {
            _rideApplicationService = rideApplicationService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Apply(CreateRideApplicationDto dto)
        {
            try
            {
                var result = await _rideApplicationService.ApplyAsync(CurrentUserId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> Accept(Guid id)
        {
            try
            {
                await _rideApplicationService.AcceptAsync(CurrentUserId, id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                await _rideApplicationService.RejectAsync(CurrentUserId, id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var result = await _rideApplicationService.GetMyApplicationsAsync(CurrentUserId);
            return Ok(result);
        }

        [HttpGet("incoming")]
        public async Task<IActionResult> GetIncoming()
        {
            var result = await _rideApplicationService.GetIncomingApplicationsAsync(CurrentUserId);
            return Ok(result);
        }
    }
}
