using Orbit.Domain.Entities;

namespace Orbit.Application.DTOs
{
    public class CurriculumParseRequest
    {
        public string ExtractedText { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
    }

    public class CurriculumParseResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public List<Course> Courses { get; set; } = new();
        public int TotalFound { get; set; }
        public int HighConfidence { get; set; }
        public int LowConfidence { get; set; }
        public DateTime ParsedAt { get; set; } = DateTime.UtcNow;
    }
}