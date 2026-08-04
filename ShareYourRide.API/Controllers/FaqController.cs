using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;
        public FaqController(IFaqService faqService) => _faqService = faqService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _faqService.GetAllAsync();
            return Ok(result);
        }
    }
}
