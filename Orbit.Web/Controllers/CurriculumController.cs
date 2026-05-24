using Microsoft.AspNetCore.Mvc;
using Orbit.Infrastructure.Pdf;

namespace Orbit.Web.Controllers
{
    [Route("api/curriculum")]
    [ApiController]
    public class CurriculumController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly PdfExtractorService _pdfService;

        public CurriculumController(IWebHostEnvironment env, PdfExtractorService pdfService)
        {
            _env = env;
            _pdfService = pdfService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 🔥 FULL extraction (no truncation)
            var extractedText = PdfExtractorService.ExtractText(filePath);

            return Ok(new
            {
                message = "Upload + extraction successful",
                fileName = file.FileName,
                content = extractedText   // 👈 FULL TEXT HERE
            });
        }
    }
    }
