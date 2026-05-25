using System.Text.RegularExpressions;

namespace Orbit.Application.Services.Parsing
{
    public class CourseBlock
    {
        public string RawText { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }

    public class CurriculumStructureParser
    {
        public List<CourseBlock> Parse(string fullText)
        {
            var blocks = new List<CourseBlock>();
            if (string.IsNullOrWhiteSpace(fullText)) return blocks;

            // Detect patterns like "CS-101", "CS 101", "SE-301", "IT-204" usually at the start of a line
            // Followed by the course name
            var pattern = @"^[ \t]*([A-Z]{2,4}[-\s]?\d{3,4})[ \t]+([^\r\n]+)";
            var regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var matches = regex.Matches(fullText);

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var nextMatch = i + 1 < matches.Count ? matches[i + 1] : null;

                int startIndex = match.Index;
                int length = nextMatch != null ? nextMatch.Index - startIndex : fullText.Length - startIndex;

                var rawText = fullText.Substring(startIndex, length).Trim();

                blocks.Add(new CourseBlock
                {
                    CourseCode = match.Groups[1].Value.Trim().ToUpper(),
                    CourseName = match.Groups[2].Value.Trim(),
                    RawText = rawText
                });
            }

            return blocks;
        }
    }
}
