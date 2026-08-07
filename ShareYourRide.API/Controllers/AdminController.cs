using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Application.DTOs.Admin;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("pending-users")]
        public async Task<IActionResult> GetActionResultAsync()
        { 
            var pendingUsers = await _adminService.GetPendingUsersAsync();
            return Ok(pendingUsers);
        }

        [HttpPost("approve-user")]
        public async Task<IActionResult> Approve(ApproveUserDto dto)
        {
            try
            {
                await _adminService.ApproveUserAsync(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reject")]
        public async Task<IActionResult> Reject(RejectUserDto dto)
        {
            try
            {
                await _adminService.RejectUserAsync(dto);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
