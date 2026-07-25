using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Application.DTOs.Wallet;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Infrastructure.Services.Implementations;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("top-up")]
        public async Task<IActionResult> TopUp(TopUpDto dto)
        {
            try
            {
                await _walletService.TopUpAsync(CurrentUserId, dto);
                return Ok(new { message = "Wallet topped up successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var result = await _walletService.GetBalanceAsync(CurrentUserId);
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var result = await _walletService.GetTransactionsAsync(CurrentUserId);
            return Ok(result);
        }
    }
}
