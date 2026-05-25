using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orbit.Domain.Entities;

namespace Orbit.Infrastructure.AI
{
    public class GeminiCurriculumParsingService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<GeminiCurriculumParsingService> _logger;

        private static readonly SemaphoreSlim _gate = new(1, 1);
        private const int InterCallDelayMs = 2_500;
        private const int MaxRetries = 3;
        private static readonly int[] BackoffMs = [2_000, 4_000, 8_000];

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        public GeminiCurriculumParsingService(
            HttpClient http,
            IConfiguration config,
            ILogger<GeminiCurriculumParsingService> logger)
        {
            _http = http;
            _logger = logger;
            _apiKey = config.GetValue<string>("AiParsing:ApiKey")
                      ?? throw new InvalidOperationException("AiParsing:ApiKey not configured.");
            _model = config["AiParsing:Model"] ?? "gemini-2.0-flash";
        }

        public async Task<List<Course>> ParseCoursesAsync(
            string extractedText,
            string universityName,
            string fileName,
            CancellationToken ct = default)
        {
            var allCourses = new List<Course>();

            // Chunk text into ~3000 char blocks to stay within token limits
            var chunks = ChunkText(extractedText, 3000);
            _logger.LogInformation("[Gemini] '{File}' split into {Count} chunk(s).", fileName, chunks.Count);

            for (int i = 0; i < chunks.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var raw = await CallWithRetryAsync(universityName, chunks[i], i + 1, ct);
                    var courses = DeserializeCourses(raw, universityName);
                    allCourses.AddRange(courses);

                    _logger.LogInformation("[Gemini] Chunk {I}/{Total} → {Count} course(s).",
                        i + 1, chunks.Count, courses.Count);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "[Gemini] Chunk {I} failed.", i + 1);
                }

                if (i < chunks.Count - 1)
                    await Task.Delay(InterCallDelayMs, ct);
            }

            // Deduplicate
            return allCourses
                .GroupBy(c => (c.CourseCode ?? "") + "|" + (c.CourseName ?? ""))
                .Select(g => g.First())
                .ToList();
        }

        private async Task<string> CallWithRetryAsync(
            string universityName, string chunk, int index, CancellationToken ct)
        {
            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                await _gate.WaitAsync(ct);
                try
                {
                    return await CallGeminiAsync(universityName, chunk, index, ct);
                }
                catch (GeminiRateLimitException) when (attempt < MaxRetries)
                {
                    int delay = BackoffMs[attempt];
                    _logger.LogWarning("[Gemini] 429 chunk {Index} attempt {A}. Retry in {D}ms.",
                        index, attempt + 1, delay);
                    await Task.Delay(delay, ct);
                }
                finally { _gate.Release(); }
            }
            throw new GeminiRateLimitException($"Chunk {index}: all retries exhausted.");
        }

        private async Task<string> CallGeminiAsync(
            string universityName, string chunk, int index, CancellationToken ct)
        {
            var url = $"https://generativelanguage.googleapis.com/v1/models/{_model}" +
                      $":generateContent?key={_apiKey}";

            var prompt = $"""
    You are a CS curriculum analyst for Pakistani universities.
    Extract ALL courses from the text below and return ONLY a JSON array.
    No markdown fences. No explanation.

    Each item:
    {"{"}
      "CourseName": "",
      "CourseCode": "",
      "Semester": 0,
      "CreditHours": "",
      "Topics": [],
      "Skills": [],
      "DifficultyLevel": "",
      "LearningOutcomes": [],
      "Prerequisites": []
    {"}"}

    University: {universityName}
    Text:
    ---
    {chunk}
    ---
    """;

            var payload = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { temperature = 0.1, maxOutputTokens = 4096 }
            };

            var response = await _http.PostAsync(url,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
                ct);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                throw new GeminiRateLimitException($"Chunk {index}: HTTP 429.");

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? throw new InvalidOperationException("Empty Gemini response.");

            return StripFences(text);
        }

        private List<Course> DeserializeCourses(string json, string universityName)
        {
            try
            {
                var courses = JsonSerializer.Deserialize<List<Course>>(json, _jsonOpts) ?? new();
                foreach (var c in courses) c.University = universityName;
                return courses;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[Gemini] Deserialize failed: {Err}", ex.Message);
                return new();
            }
        }

        private static List<string> ChunkText(string text, int chunkSize)
        {
            var chunks = new List<string>();
            for (int i = 0; i < text.Length; i += chunkSize)
                chunks.Add(text.Substring(i, Math.Min(chunkSize, text.Length - i)));
            return chunks;
        }

        private static string StripFences(string s)
        {
            s = s.Trim();
            if (s.StartsWith("```json", StringComparison.OrdinalIgnoreCase)) s = s[7..];
            else if (s.StartsWith("```")) s = s[3..];
            if (s.EndsWith("```")) s = s[..^3];
            return s.Trim();
        }
    }

    public sealed class GeminiRateLimitException : Exception
    {
        public GeminiRateLimitException(string message) : base(message) { }
    }
}