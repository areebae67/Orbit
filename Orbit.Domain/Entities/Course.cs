namespace Orbit.Domain.Entities
{
    public class Course
    {
        public string CourseName { get; set; }
        public string? CourseCode { get; set; }

        public int Semester { get; set; }

        public string? CreditHours { get; set; }

        public List<string> Prerequisites { get; set; } = new();

        public string? University { get; set; }

        public string? DegreeProgram { get; set; }

        // CORE INTELLIGENCE LAYER (EMPTY FOR NOW)
        public List<string> Topics { get; set; } = new();
        public List<string> Concepts { get; set; } = new();
        public List<string> Skills { get; set; } = new();

        public string? DifficultyLevel { get; set; }

        public List<string> LearningOutcomes { get; set; } = new();

        public List<string> CognitiveLevels { get; set; } = new();
    }
}