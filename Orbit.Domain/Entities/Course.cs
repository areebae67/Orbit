namespace Orbit.Domain.Entities
{
    public class Course
    {
        // Existing fields — unchanged
        public string CourseName { get; set; } = string.Empty;
        public string? CourseCode { get; set; }
        public int Semester { get; set; }
        public string? CreditHours { get; set; }
        public List<string> Prerequisites { get; set; } = new();
        public string? University { get; set; }
        public string? DegreeProgram { get; set; }
        public List<string> Topics { get; set; } = new();
        public List<string> Concepts { get; set; } = new();
        public List<string> Skills { get; set; } = new();
        public string? DifficultyLevel { get; set; }
        public List<string> LearningOutcomes { get; set; } = new();
        public List<string> CognitiveLevels { get; set; } = new();

        // AI-populated gap analysis fields
        public int DepthRating { get; set; }        // 1–5
        public int BreadthRating { get; set; }      // 1–5
        public int PracticalBalance { get; set; }   // 1–5
        public bool HasDedicatedLab { get; set; }
        public string? AssessmentStyle { get; set; }
        public string? ProjectScope { get; set; }
        public string? ParseConfidence { get; set; }
        public string? ParseNotes { get; set; }
    }
}