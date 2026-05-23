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
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            // 1. Validate file
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!file.FileName.EndsWith(".pdf"))
                return BadRequest("Only PDF files allowed");

            // 2. Create folder if not exists
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads/pdfs");
            Directory.CreateDirectory(uploadPath);

            // 3. Save file
            var filePath = Path.Combine(uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. Extract text using PdfPig
            var extractedText = _pdfService.ExtractText(filePath);

            // 5. Return preview (first 500 chars)
            return Ok(new
            {
                message = "Upload + extraction successful",
                fileName = file.FileName,
                preview = extractedText.Length > 500
                    ? extractedText.Substring(0, 500)
                    : extractedText
            });
        }
    }
}