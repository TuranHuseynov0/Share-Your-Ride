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
                var balance = await _walletService.GetBalanceAsync(CurrentUserId);
                return Ok(balance);   // { balance: ... } — frontend həmişə eyni formatı alır
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Balans artırılarkən xəta baş verdi." });
            }
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var result = await _walletService.GetBalanceAsync(CurrentUserId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            try
            {
                var result = await _walletService.GetTransactionsAsync(CurrentUserId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
