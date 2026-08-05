using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace ShareYourRide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        // yalnız bu bölmələrə yükləməyə icazə verilir
        private static readonly HashSet<string> AllowedFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            "profile",
            "vehicles"
        };

        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder, [FromQuery] Guid? userId = null)
        {
            if (!AllowedFolders.Contains(folder))
                return BadRequest(new { message = "Bu bölməyə fayl yükləməyə icazə yoxdur." }); //[cite: 2]

            try
            {
                // Əgər userId parametr kimi gəlməyibsə, Claim-dən oxumağa çalış
                var targetUserId = userId ?? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

                if (targetUserId == Guid.Empty)
                    return Unauthorized(new { message = "İstifadəçi tapılmadı. Zəhmət olmasa userId göndərin və ya sistemə daxil olun." });

                var scopedFolder = $"{folder}/{targetUserId}"; //[cite: 2]
                var path = await _fileStorageService.SaveFileAsync(file, scopedFolder); //[cite: 2]

                return Ok(new { path }); //[cite: 2]
            }
            catch (InvalidOperationException ex) //[cite: 2]
            {
                return BadRequest(new { message = ex.Message }); //[cite: 2]
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Fayl yüklənərkən xəta baş verdi." });
            }
        }
    }
}