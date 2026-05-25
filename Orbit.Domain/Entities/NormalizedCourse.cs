

namespace Orbit.Domain.Entities
{
    /// <summary>
    /// Layer 6: Normalized course entity — ONE schema for all universities.
    /// Every course from NUML, PIEAS, COMSATS maps to this.
    /// </summary>
    public class NormalizedCourse
    {
        public int Id { get; set; }
        public int UniversityId { get; set; }

        // Identity
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Semester { get; set; }
        public int CreditHours { get; set; }
        public int TheoryHours { get; set; }
        public int LabHours { get; set; }
        public bool HasLab { get; set; }

        // Content (from Layer 4)
        public string TopicsJson { get; set; } = "[]";
        public string PrerequisitesJson { get; set; } = "[]";
        public string LearningOutcomesJson { get; set; } = "[]";
        public string SkillTagsJson { get; set; } = "[]";

        // Gap analysis dimensions (from Layer 5 AI)
        public int DepthRating { get; set; }   // 1-5
        public int BreadthRating { get; set; }   // 1-5
        public int PracticalBalance { get; set; }   // 1-5
        public string AssessmentStyle { get; set; } = "Unknown";
        public string DifficultyLevel { get; set; } = "Intermediate";

        // Embedding for comparison (Layer 7)
        public string? EmbeddingJson { get; set; }   // stored as JSON float array

        // Metadata
        public string SourceFormat { get; set; } = string.Empty;
        public bool IsAdminVerified { get; set; }
        public DateTime IngestedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public University? University { get; set; }
    

    /// <summary>
    /// Maps StructuredCourseBlock → NormalizedCourse
    /// </summary>
    
        private static string Serialize(List<string> list) =>
            System.Text.Json.JsonSerializer.Serialize(list);
    }
}
