using Microsoft.Extensions.Logging;
using Orbit.Domain.Entities;
using Orbit.Infrastructure.AI;
using Orbit.Infrastructure.Pdf;

namespace Orbit.Infrastructure.Services
{
    public class CurriculumIntelligenceEngine
    {
        private readonly CurriculumTextProcessor _textProcessor;
        private readonly StructureInferenceEngine _structureEngine;
        private readonly CourseBlockBuilder _blockBuilder;
        private readonly AiEnrichmentService _aiEnrichment;
        private readonly ILogger<CurriculumIntelligenceEngine> _logger;

        public CurriculumIntelligenceEngine(
            CurriculumTextProcessor textProcessor,
            StructureInferenceEngine structureEngine,
            CourseBlockBuilder blockBuilder,
            AiEnrichmentService aiEnrichment,
            ILogger<CurriculumIntelligenceEngine> logger)
        {
            _textProcessor = textProcessor;
            _structureEngine = structureEngine;
            _blockBuilder = blockBuilder;
            _aiEnrichment = aiEnrichment;
            _logger = logger;
        }

        public async Task<PipelineResult> ProcessAsync(
            string pdfFilePath,
            int universityId,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Pipeline start: {File}", Path.GetFileName(pdfFilePath));

            // Layer 1+2: Extract & Clean
            var extraction = _textProcessor.ExtractAndClean(pdfFilePath);

            // Layer 3: Structure Inference
            var structure = _structureEngine.Infer(extraction.CleanedText);
            _logger.LogInformation("Format: {Format}, Confidence: {Score}%, Blocks: {Count}",
                structure.Format, structure.ConfidenceScore, structure.CourseBlocks.Count);

            if (structure.CourseBlocks.Count == 0)
                return new PipelineResult
                {
                    Success = false,
                    Error = $"No courses found. Format: {structure.Format}"
                };

            // Layer 4: Course Block Builder
            var blocks = _blockBuilder.Build(structure.CourseBlocks, structure.SemesterBoundaries);

            // Layer 5: AI Enrichment
            await _aiEnrichment.EnrichAsync(blocks, ct);

            // Layer 6: Normalize
            var courses = blocks
                .Select(b => CourseMapper.ToNormalized(b, universityId))
                .ToList();

            return new PipelineResult
            {
                Success = true,
                Courses = courses,
                TotalCourses = courses.Count,
                SemestersDetected = structure.SemesterBoundaries.Count,
                FormatDetected = structure.Format.ToString(),
                StructureConfidence = structure.ConfidenceScore,
                HasOcrArtifacts = extraction.HasOcrArtifacts
            };
        }
    }

    public class PipelineResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public List<NormalizedCourse> Courses { get; set; } = new();
        public int TotalCourses { get; set; }
        public int SemestersDetected { get; set; }
        public string FormatDetected { get; set; } = string.Empty;
        public int StructureConfidence { get; set; }
        public bool HasOcrArtifacts { get; set; }
    }
}