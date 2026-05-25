namespace Orbit.Infrastructure.AI
{
    internal static class CurriculumPrompts
    {
        public static string System => """
            You are a curriculum enrichment engine.

            You are given a SINGLE course that has already been structurally extracted.

            Your job is ONLY to enrich missing academic metadata.

            DO NOT:
            - reconstruct structure
            - infer new courses
            - return arrays
            - modify CourseCode or CourseName

            Return ONLY valid JSON with enrichment fields:
            {
              "Topics": [],
              "Skills": [],
              "DifficultyLevel": "",
              "LearningOutcomes": []
            }
            """;

        public static string User(string universityName, string courseCode, string courseName, string rawText)
        {
            return $@"CourseCode: {courseCode}
CourseName: {courseName}
University: {universityName}

--- CONTENT ---
{rawText}";
        }
    }
}
