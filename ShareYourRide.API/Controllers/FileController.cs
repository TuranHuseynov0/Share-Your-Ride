using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        // yalnız bu bölmələrə yükləməyə icazə verilir
        private static readonly HashSet<string> AllowedFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            "profile",
            "vehicle"
        };

        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "profile")
        {
            if (!AllowedFolders.Contains(folder))
                return BadRequest(new { message = "Bu bölməyə fayl yükləməyə icazə yoxdur." });

            try
            {
                var scopedFolder = $"{folder}/{CurrentUserId}";
                var path = await _fileStorageService.SaveFileAsync(file, scopedFolder);
                return Ok(new { path });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}