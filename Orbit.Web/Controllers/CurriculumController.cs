using Microsoft.AspNetCore.Mvc;
using Orbit.Infrastructure.Services;

namespace Orbit.Web.Controllers
{
    [Route("api/curriculum")]
    [ApiController]
    public class CurriculumController : ControllerBase
    {
        private readonly CurriculumIntelligenceEngine _pipeline;
        private readonly CurriculumComparisonEngine _comparison;
        private readonly ILogger<CurriculumController> _logger;

        public CurriculumController(
            CurriculumIntelligenceEngine pipeline,
            CurriculumComparisonEngine comparison,
            ILogger<CurriculumController> logger)
        {
            _pipeline = pipeline;
            _comparison = comparison;
            _logger = logger;
        }

        [HttpPost("debug-structure")]
        public async Task<IActionResult> DebugStructure(IFormFile file)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            // Test on RAW text (no cleaning)
            var rawText = string.Empty;
            using (var doc = UglyToad.PdfPig.PdfDocument.Open(filePath))
            {
                var sb = new System.Text.StringBuilder();
                foreach (var page in doc.GetPages())
                    sb.AppendLine(page.Text);
                rawText = sb.ToString();
            }

            // Test on raw text directly
            var engine = new Orbit.Infrastructure.Pdf.StructureInferenceEngine();
            var structureRaw = engine.Infer(rawText);

            // Test on cleaned text
            var processor = new Orbit.Infrastructure.Pdf.CurriculumTextProcessor();
            var extraction = processor.ExtractAndClean(filePath);
            var structureCleaned = engine.Infer(extraction.CleanedText);

            return Ok(new
            {
                raw = new
                {
                    format = structureRaw.Format.ToString(),
                    blocksFound = structureRaw.CourseBlocks.Count,
                    semesters = structureRaw.SemesterBoundaries.Count,
                    firstBlock = structureRaw.CourseBlocks.FirstOrDefault()?.CourseTitle
                },
                cleaned = new
                {
                    format = structureCleaned.Format.ToString(),
                    blocksFound = structureCleaned.CourseBlocks.Count,
                    semesters = structureCleaned.SemesterBoundaries.Count,
                    firstBlock = structureCleaned.CourseBlocks.FirstOrDefault()?.CourseTitle
                },
                cleanedTextLength = extraction.CleanedText.Length,
                courseCodeInRaw = System.Text.RegularExpressions.Regex.IsMatch(rawText, @"Course Code\s*:\s*[A-Z]{2,6}[-\s]\d{3,4}", System.Text.RegularExpressions.RegexOptions.IgnoreCase),
                courseCodeInCleaned = System.Text.RegularExpressions.Regex.IsMatch(extraction.CleanedText, @"Course Code\s*:\s*[A-Z]{2,6}[-\s]\d{3,4}", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            });
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process(
            IFormFile file,
            [FromForm] int universityId,
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            if (universityId <= 0)
                return BadRequest("universityId required.");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, $"{universityId}_{file.FileName}");

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream, ct);

            var result = await _pipeline.ProcessAsync(filePath, universityId, ct);

            if (!result.Success)
                return UnprocessableEntity(new { error = result.Error });

            return Ok(result);
        }

        [HttpPost("compare")]
        public IActionResult Compare([FromBody] CompareRequest request)
        {
            var report = _comparison.Compare(request.Source, request.Target);
            return Ok(report);
        }
    }

    public class CompareRequest
    {
        public UniversityCurriculum Source { get; set; } = new();
        public UniversityCurriculum Target { get; set; } = new();
    }
}