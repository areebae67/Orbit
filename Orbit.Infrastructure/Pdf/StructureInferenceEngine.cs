using System.Text.RegularExpressions;

namespace Orbit.Infrastructure.Pdf
{
    public class StructureInferenceEngine
    {
        public StructureInferenceResult Infer(string cleanedText)
        {
            var result = new StructureInferenceResult();
            result.Format = DetectFormat(cleanedText);
            result.SemesterBoundaries = FindSemesterBoundaries(cleanedText);
            result.CourseBlocks = ExtractCourseBlocks(cleanedText, result.Format);
            AssignSemesters(result.CourseBlocks, result.SemesterBoundaries, cleanedText);
            result.ConfidenceScore = ComputeConfidence(result);
            return result;
        }

        private static CurriculumFormat DetectFormat(string text)
        {
            if (Regex.IsMatch(text, @"Course Code\s*:\s*[A-Z]{2,6}[-\s]\d{3,4}", RegexOptions.IgnoreCase))
                return CurriculumFormat.Booklet;

            if (Regex.IsMatch(text, @"[A-Z]{2,6}-\d{3,4}[L]?\s+.{10,60}\s+\d-\d"))
                return CurriculumFormat.SchemeTable;

            if (Regex.IsMatch(text, @"[A-Z]{2,6}-\d{3,4}"))
                return CurriculumFormat.SimpleList;

            return CurriculumFormat.Unknown;
        }

        private static List<SemesterBoundary> FindSemesterBoundaries(string text)
        {
            var boundaries = new List<SemesterBoundary>();
            var lines = text.Split('\n');

            var semPattern = new Regex(
                @"^\s*Semester\s*[–\-]?\s*(I{1,3}V?|VI{0,3}|VIII|VII|V|\d{1,2})\s*$",
                RegexOptions.IgnoreCase);

            for (int i = 0; i < lines.Length; i++)
            {
                var m = semPattern.Match(lines[i].Trim());
                if (m.Success)
                {
                    boundaries.Add(new SemesterBoundary
                    {
                        Label = lines[i].Trim(),
                        LineIndex = i,
                        SemesterNumber = ParseRoman(m.Groups[1].Value)
                    });
                }
            }

            return boundaries;
        }

        private static List<RawCourseBlock> ExtractCourseBlocks(
            string text, CurriculumFormat format)
        {
            return format switch
            {
                CurriculumFormat.Booklet => ExtractFromBooklet(text),
                CurriculumFormat.SchemeTable => ExtractFromTable(text),
                CurriculumFormat.SimpleList => ExtractFromTable(text),
                _ => ExtractFromBooklet(text)
            };
        }

        private static List<RawCourseBlock> ExtractFromBooklet(string text)
        {
            var blocks = new List<RawCourseBlock>();
            var lines = text.Split('\n');

            // Pattern 1: Booklet — title on one line, "Course Code: CSIT-107" on next
            var bookletPattern = new Regex(
                @"(?<title>[A-Z][A-Za-z\s&/,\-().']{5,100}?)\s*\n\s*Course Code\s*:\s*(?<code>[A-Z]{2,6}[-\s]\d{3,4}L?)",
                RegexOptions.Multiline);

            var matches = bookletPattern.Matches(text);

            for (int i = 0; i < matches.Count; i++)
            {
                var start = matches[i].Index;
                var end = i + 1 < matches.Count ? matches[i + 1].Index : text.Length;

                // Skip table-of-contents entries (very little surrounding text)
                if (end - start < 100 && i < matches.Count - 1)
                    continue;

                // Compute approximate line index for semester assignment
                var lineIndex = text[..start].Count(c => c == '\n');

                blocks.Add(new RawCourseBlock
                {
                    CourseTitle = matches[i].Groups["title"].Value.Trim(),
                    CourseCode = matches[i].Groups["code"].Value.Trim().Replace(" ", "-"),
                    FullText = text[start..Math.Min(end, start + 4000)],
                    Format = CurriculumFormat.Booklet,
                    LineIndex = lineIndex
                });
            }

            // Pattern 2: Scheme table rows as fallback
            if (blocks.Count == 0)
            {
                var tablePattern = new Regex(
                    @"(?<code>[A-Z]{2,6}-\d{3,4}L?)\s+(?<title>[A-Za-z][A-Za-z\s&/,\-().']{5,70}?)\s+(?<credits>\d-\d|\d\+\d)",
                    RegexOptions.Multiline);

                foreach (Match m in tablePattern.Matches(text))
                {
                    var lineIndex = text[..m.Index].Count(c => c == '\n');
                    blocks.Add(new RawCourseBlock
                    {
                        CourseCode = m.Groups["code"].Value.Trim(),
                        CourseTitle = m.Groups["title"].Value.Trim(),
                        CreditHours = m.Groups["credits"].Value.Trim(),
                        FullText = m.Value,
                        Format = CurriculumFormat.SchemeTable,
                        LineIndex = lineIndex
                    });
                }
            }

            return blocks;
        }

        private static List<RawCourseBlock> ExtractFromTable(string text)
        {
            var blocks = new List<RawCourseBlock>();

            var rowPattern = new Regex(
                @"(?<code>[A-Z]{2,6}-\d{3,4}L?)\s+(?<title>[A-Za-z][A-Za-z\s&/,\-().']{5,70}?)\s+(?<credits>\d[-–+]\d|\d)\s",
                RegexOptions.Multiline);

            foreach (Match m in rowPattern.Matches(text))
            {
                var lineIndex = text[..m.Index].Count(c => c == '\n');
                blocks.Add(new RawCourseBlock
                {
                    CourseCode = m.Groups["code"].Value.Trim(),
                    CourseTitle = m.Groups["title"].Value.Trim(),
                    CreditHours = m.Groups["credits"].Value.Trim(),
                    FullText = m.Value,
                    Format = CurriculumFormat.SchemeTable,
                    LineIndex = lineIndex
                });
            }

            return blocks;
        }

        // Assign semester numbers based on which boundary comes before each block
        private static void AssignSemesters(
            List<RawCourseBlock> blocks,
            List<SemesterBoundary> boundaries,
            string text)
        {
            if (!boundaries.Any()) return;

            foreach (var block in blocks)
            {
                var closest = boundaries
                    .Where(b => b.LineIndex <= block.LineIndex)
                    .OrderByDescending(b => b.LineIndex)
                    .FirstOrDefault();

                block.DetectedSemester = closest?.SemesterNumber ?? 0;
            }
        }

        private static int ComputeConfidence(StructureInferenceResult r)
        {
            if (r.Format == CurriculumFormat.Unknown) return 20;
            if (r.CourseBlocks.Count == 0) return 30;
            if (r.CourseBlocks.Count < 5) return 50;
            if (r.SemesterBoundaries.Count > 0) return 90;
            return 70;
        }

        private static int ParseRoman(string value)
        {
            if (int.TryParse(value, out var n)) return n;
            return value.ToUpperInvariant() switch
            {
                "I" => 1,
                "II" => 2,
                "III" => 3,
                "IV" => 4,
                "V" => 5,
                "VI" => 6,
                "VII" => 7,
                "VIII" => 8,
                _ => 0
            };
        }
    }

    public enum CurriculumFormat { Booklet, SchemeTable, SimpleList, Unknown }

    public class StructureInferenceResult
    {
        public CurriculumFormat Format { get; set; }
        public List<SemesterBoundary> SemesterBoundaries { get; set; } = new();
        public List<RawCourseBlock> CourseBlocks { get; set; } = new();
        public int ConfidenceScore { get; set; }
    }

    public class SemesterBoundary
    {
        public string Label { get; set; } = string.Empty;
        public int LineIndex { get; set; }
        public int SemesterNumber { get; set; }
    }

    public class RawCourseBlock
    {
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CreditHours { get; set; } = string.Empty;
        public string FullText { get; set; } = string.Empty;
        public CurriculumFormat Format { get; set; }
        public int DetectedSemester { get; set; }
        public int LineIndex { get; set; }  // for semester boundary matching
    }
}