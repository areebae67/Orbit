using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Orbit.Infrastructure.AI
{
    /// <summary>
    /// Layer 5: AI Enrichment
    /// ONLY enriches fields that pattern matching cannot determine:
    ///   - DepthRating (1-5)
    ///   - BreadthRating (1-5)
    ///   - PracticalBalance (1-5)
    ///   - DifficultyLevel
    ///   - SkillTags (industry-relevant skills)
    /// 
    /// Batches up to 10 courses per Gemini call to minimize cost.
    /// Topics, CLOs, credits, semester — all done by Layer 4, never by AI.
    /// </summary>
    public class AiEnrichmentService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<AiEnrichmentService> _logger;

        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        private const string SystemPrompt = """
            You are a CS curriculum analyst. Given a list of courses with their topics,
            rate each course and return ONLY a JSON array. No explanation. No markdown.

            For each course, return:
            {
              "courseCode": "...",
              "depthRating": 1-5,
              "breadthRating": 1-5,
              "practicalBalance": 1-5,
              "difficultyLevel": "Introductory|Intermediate|Advanced",
              "skillTags": ["tag1","tag2"]
            }

            DepthRating: 1=surface intro, 3=standard undergrad, 5=grad/competitive
            BreadthRating: 1=single topic, 3=4-6 topics, 5=10+ topics
            PracticalBalance: 1=pure theory, 3=balanced, 5=project/lab heavy
            SkillTags: industry-relevant skills only (e.g. "DSA","OOP","SQL","REST APIs")
            """;

        public AiEnrichmentService(
            HttpClient http, IConfiguration config,
            ILogger<AiEnrichmentService> logger)
        {
            _http = http;
            _apiKey = config["AiParsing:ApiKey"]
                       ?? throw new InvalidOperationException("AiParsing:ApiKey missing");
            _model = config["AiParsing:Model"] ?? "gemini-1.5-flash";
            _logger = logger;
        }

        public async Task EnrichAsync(
            List<Pdf.StructuredCourseBlock> blocks,
            CancellationToken ct = default)
        {
            // Only enrich non-lab courses that need AI
            var toEnrich = blocks
                .Where(b => !b.IsLabCourse && b.NeedsAiDepthRating)
                .ToList();

            _logger.LogInformation("AI enriching {Count} courses", toEnrich.Count);

            // Batch: 10 courses per call
            foreach (var batch in toEnrich.Chunk(10))
            {
                await EnrichBatchAsync(batch, ct);
                await Task.Delay(200, ct); // Respect free tier rate limits
            }
        }

        private async Task EnrichBatchAsync(
            Pdf.StructuredCourseBlock[] batch, CancellationToken ct)
        {
            var courseSummaries = batch.Select(b => new
            {
                courseCode = b.CourseCode,
                courseTitle = b.CourseTitle,
                topics = b.Topics.Take(10),
                clos = b.LearningOutcomes.Take(3),
                hasLab = b.HasLab
            });

            var userMessage = $"Rate these courses:\n{JsonSerializer.Serialize(courseSummaries)}";

            try
            {
                var raw = await CallGeminiAsync(userMessage, ct);
                ApplyEnrichment(batch, raw);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Enrichment batch failed, using defaults");
                ApplyDefaults(batch);
            }
        }

        private async Task<string> CallGeminiAsync(string userMessage, CancellationToken ct)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = $"{SystemPrompt}\n\n{userMessage}" } } }
                },
                generationConfig = new { temperature = 0.1, maxOutputTokens = 4096 }
            };

            var response = await _http.PostAsync(url,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
                ct);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";

            return StripFences(text);
        }

        private static void ApplyEnrichment(
            Pdf.StructuredCourseBlock[] batch, string json)
        {
            try
            {
                var results = JsonSerializer.Deserialize<List<EnrichmentResult>>(json, _json);
                if (results == null) return;

                foreach (var r in results)
                {
                    var block = batch.FirstOrDefault(b =>
                        b.CourseCode.Equals(r.CourseCode, StringComparison.OrdinalIgnoreCase));
                    if (block == null) continue;

                    block.DepthRating = Clamp(r.DepthRating);
                    block.BreadthRating = Clamp(r.BreadthRating);
                    block.PracticalBalance = Clamp(r.PracticalBalance);
                    block.DifficultyLevel = r.DifficultyLevel ?? "Intermediate";
                    block.SkillTags = r.SkillTags ?? new();
                    block.NeedsAiDepthRating = false;
                }
            }
            catch { ApplyDefaults(batch); }
        }

        private static void ApplyDefaults(Pdf.StructuredCourseBlock[] batch)
        {
            foreach (var b in batch)
            {
                b.DepthRating = 2;
                b.BreadthRating = 3;
                b.PracticalBalance = b.HasLab ? 3 : 2;
                b.DifficultyLevel = "Intermediate";
                b.NeedsAiDepthRating = false;
            }
        }

        private static int Clamp(int v) => Math.Clamp(v, 1, 5);

        private static string StripFences(string s)
        {
            s = s.Trim();
            if (s.StartsWith("```json", StringComparison.OrdinalIgnoreCase)) s = s[7..];
            else if (s.StartsWith("```")) s = s[3..];
            if (s.EndsWith("```")) s = s[..^3];
            return s.Trim();
        }

        private class EnrichmentResult
        {
            public string? CourseCode { get; set; }
            public int DepthRating { get; set; } = 2;
            public int BreadthRating { get; set; } = 3;
            public int PracticalBalance { get; set; } = 2;
            public string? DifficultyLevel { get; set; }
            public List<string>? SkillTags { get; set; }
        }
    }
}
