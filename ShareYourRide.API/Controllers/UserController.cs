using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Application.DTOs.User;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var result = await _userService.GetProfileAsync(CurrentUserId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto dto)
        {
            try
            {
                var result = await _userService.UpdateProfileAsync(CurrentUserId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}