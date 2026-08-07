using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Application.DTOs.Chat;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("threads")]
        public async Task<IActionResult> GetThreads()
        {
            var result = await _chatService.GetMyThreadsAsync(CurrentUserId);
            return Ok(result);
        }

        [HttpGet("threads/{id}/messages")]
        public async Task<IActionResult> GetMessages(Guid id)
        {
            try
            {
                var result = await _chatService.GetMessagesAsync(CurrentUserId, id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("threads/{id}/messages")]
        public async Task<IActionResult> SendMessage(Guid id, SendMessageDto dto)
        {
            try
            {
                var result = await _chatService.SendMessageAsync(CurrentUserId, id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}