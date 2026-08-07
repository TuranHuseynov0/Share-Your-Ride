using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleCatalogController : ControllerBase
    {
        private readonly IVehicleCatalogService _vehicleCatalogService;

        public VehicleCatalogController(IVehicleCatalogService vehicleCatalogService)
        {
            _vehicleCatalogService = vehicleCatalogService;
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _vehicleCatalogService.GetBrandsAsync();
            return Ok(result);
        }

        [HttpGet("brands/{brandId}/models")]
        public async Task<IActionResult> GetModels(Guid brandId)
        {
            var result = await _vehicleCatalogService.GetModelsByBrandAsync(brandId);
            return Ok(result);
        }

        [HttpGet("colors")]
        public async Task<IActionResult> GetColors()
        {
            var result = await _vehicleCatalogService.GetColorsAsync();
            return Ok(result);
        }
    }
}
