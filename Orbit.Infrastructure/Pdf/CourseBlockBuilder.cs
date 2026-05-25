using System.Text.RegularExpressions;

namespace Orbit.Infrastructure.Pdf
{
    public class CourseBlockBuilder
    {
        public List<StructuredCourseBlock> Build(
            List<RawCourseBlock> rawBlocks,
            List<SemesterBoundary> semesters)
        {
            var result = new List<StructuredCourseBlock>();

            foreach (var raw in rawBlocks)
            {
                var block = new StructuredCourseBlock
                {
                    CourseTitle = Normalize(raw.CourseTitle),
                    CourseCode = NormalizeCode(raw.CourseCode),
                    CreditHours = ParseCredits(raw.CreditHours),
                    TheoryHours = ParseTheoryHours(raw.CreditHours),
                    LabHours = ParseLabHours(raw.CreditHours),
                    HasLab = HasLabComponent(raw),
                    IsLabCourse = IsLabCourse(raw.CourseCode, raw.CourseTitle),
                    Semester = raw.DetectedSemester > 0
                                    ? raw.DetectedSemester
                                    : InferSemesterFromContext(raw, semesters),
                    Prerequisites = ExtractPrerequisites(raw.FullText),
                    Topics = ExtractTopics(raw.FullText),
                    LearningOutcomes = ExtractCLOs(raw.FullText),
                    AssessmentStyle = InferAssessmentStyle(raw.FullText),
                    SourceFormat = raw.Format,
                    RawBlock = raw.FullText,
                    NeedsAiDepthRating = true,
                    NeedsAiDifficultyLevel = true,
                    NeedsAiSkillTags = true
                };

                result.Add(block);
            }

            return MergeLabs(result);
        }

        private static string Normalize(string title) =>
            Regex.Replace(title.Trim(), @"\s+", " ");

        private static string NormalizeCode(string code) =>
            Regex.Replace(code.Trim().ToUpper(), @"\s+", "-");

        private static int ParseCredits(string credits)
        {
            if (string.IsNullOrWhiteSpace(credits)) return 3;
            var m = Regex.Match(credits, @"(\d+)\s*[-+]\s*(\d+)");
            if (m.Success)
                return int.Parse(m.Groups[1].Value) + int.Parse(m.Groups[2].Value);
            if (int.TryParse(credits.Trim(), out var n)) return n;
            return 3;
        }

        private static int ParseTheoryHours(string credits)
        {
            var m = Regex.Match(credits ?? "", @"(\d+)\s*[-+]\s*\d+");
            return m.Success ? int.Parse(m.Groups[1].Value) : ParseCredits(credits);
        }

        private static int ParseLabHours(string credits)
        {
            var m = Regex.Match(credits ?? "", @"\d+\s*[-+]\s*(\d+)");
            return m.Success ? int.Parse(m.Groups[1].Value) : 0;
        }

        private static bool HasLabComponent(RawCourseBlock raw) =>
            ParseLabHours(raw.CreditHours) > 0 ||
            Regex.IsMatch(raw.FullText, @"lab|laboratory|practical", RegexOptions.IgnoreCase);

        private static bool IsLabCourse(string code, string title) =>
            code.EndsWith("L") ||
            Regex.IsMatch(title, @"\blab\b|\blaboratory\b", RegexOptions.IgnoreCase);

        // Uses LineIndex on RawCourseBlock for accurate semester assignment
        private static int InferSemesterFromContext(
            RawCourseBlock raw, List<SemesterBoundary> boundaries)
        {
            if (!boundaries.Any()) return 0;
            return boundaries
                .Where(b => b.LineIndex <= raw.LineIndex)
                .OrderByDescending(b => b.LineIndex)
                .FirstOrDefault()?.SemesterNumber ?? 0;
        }

        private static List<string> ExtractPrerequisites(string text)
        {
            var m = Regex.Match(text,
                @"Pre[-\s]?[Rr]equisite[s]?\s*[:\|]\s*(?<prereq>[^\n\r]{1,200})",
                RegexOptions.IgnoreCase);
            if (!m.Success) return new();

            var raw = m.Groups["prereq"].Value.Trim();
            if (raw.Equals("None", StringComparison.OrdinalIgnoreCase)) return new();

            return raw.Split(new[] { ',', '/', '&' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(p => p.Trim())
                      .Where(p => p.Length > 1)
                      .ToList();
        }

        private static List<string> ExtractTopics(string text)
        {
            var topics = new List<string>();

            var materialsSection = Regex.Match(text,
                @"Course Materials\s*\n(?<body>[\s\S]{50,2000}?)(?=Course Weekly Schedule|Course Learning|Recommended|$)",
                RegexOptions.IgnoreCase);

            if (materialsSection.Success)
            {
                var body = materialsSection.Groups["body"].Value;
                var bullets = Regex.Matches(body, @"(?:^|\n)\s*[●\-\*•]\s*(?<item>[^\n]{5,100})");
                foreach (Match b in bullets)
                    topics.Add(b.Groups["item"].Value.Trim());

                if (!topics.Any())
                {
                    topics = body.Split(',')
                        .Select(t => t.Trim())
                        .Where(t => t.Length is > 3 and < 80)
                        .Take(20)
                        .ToList();
                }
            }

            return topics.Distinct().Take(25).ToList();
        }

        private static List<string> ExtractCLOs(string text)
        {
            var clos = new List<string>();
            var cloPattern = new Regex(
                @"CLO[-\s]*\d+\s+(?<statement>[A-Z][^\n]{20,200})",
                RegexOptions.IgnoreCase);

            foreach (Match m in cloPattern.Matches(text))
                clos.Add(m.Groups["statement"].Value.Trim());

            return clos;
        }

        private static string InferAssessmentStyle(string text)
        {
            var lower = text.ToLower();
            if (lower.Contains("mcq") || lower.Contains("multiple choice")) return "McqHeavy";
            if (lower.Contains("project") && lower.Contains("lab")) return "ProjectBased";
            if (lower.Contains("timed") || lower.Contains("coding test")) return "TimedCoding";
            if (lower.Contains("problem solving") || lower.Contains("assignment")) return "ProblemSolving";
            return "Mixed";
        }

        private static List<StructuredCourseBlock> MergeLabs(List<StructuredCourseBlock> blocks)
        {
            var theory = blocks.Where(b => !b.IsLabCourse).ToList();
            var labs = blocks.Where(b => b.IsLabCourse).ToList();

            foreach (var lab in labs)
            {
                var theoryCode = lab.CourseCode.TrimEnd('L');
                var match = theory.FirstOrDefault(t =>
                    t.CourseCode == theoryCode ||
                    Similarity(t.CourseTitle, lab.CourseTitle) > 0.7);

                if (match != null)
                    match.HasLab = true;
                else
                    theory.Add(lab);
            }

            return theory;
        }

        private static double Similarity(string a, string b)
        {
            var wa = a.ToLower().Split(' ').ToHashSet();
            var wb = b.ToLower().Split(' ').ToHashSet();
            var intersection = wa.Intersect(wb).Count();
            return (double)intersection / Math.Max(wa.Count, wb.Count);
        }
    }

    public class StructuredCourseBlock
    {
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int TheoryHours { get; set; }
        public int LabHours { get; set; }
        public bool HasLab { get; set; }
        public bool IsLabCourse { get; set; }
        public int Semester { get; set; }
        public List<string> Prerequisites { get; set; } = new();
        public List<string> Topics { get; set; } = new();
        public List<string> LearningOutcomes { get; set; } = new();
        public string AssessmentStyle { get; set; } = "Unknown";
        public CurriculumFormat SourceFormat { get; set; }
        public string RawBlock { get; set; } = string.Empty;
        public bool NeedsAiDepthRating { get; set; }
        public bool NeedsAiDifficultyLevel { get; set; }
        public bool NeedsAiSkillTags { get; set; }
        public int DepthRating { get; set; }
        public int BreadthRating { get; set; }
        public int PracticalBalance { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public List<string> SkillTags { get; set; } = new();
    }
}