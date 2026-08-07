using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StopController : ControllerBase
    {
        private readonly IStopService _stopService;

        public StopController(IStopService stopService)
        {
            _stopService = stopService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStops()
        {
            var stops = await _stopService.GetAllAsync();
            return Ok(stops);
        }
    }
}
