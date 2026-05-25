using System.Text.Json;
using Orbit.Domain.Entities;

namespace Orbit.Infrastructure.Services
{
    public class CurriculumComparisonEngine
    {
        public CurriculumComparisonReport Compare(
            UniversityCurriculum source,
            UniversityCurriculum target)
        {
            var report = new CurriculumComparisonReport
            {
                SourceUniversity = source.UniversityName,
                TargetUniversity = target.UniversityName,
                GeneratedAt = DateTime.UtcNow
            };

            report.CourseOverlap = ComputeCourseOverlap(source.Courses, target.Courses);
            report.DimensionGaps = ComputeDimensionGaps(source.Courses, target.Courses);
            report.MissingTopics = FindMissingTopics(source.Courses, target.Courses);
            report.CreditMapping = MapCredits(source.Courses, target.Courses);
            report.OverallGapScore = ComputeOAGS(report.DimensionGaps);
            report.Severity = ClassifySeverity(report.OverallGapScore);

            return report;
        }

        private static List<CourseMatch> ComputeCourseOverlap(
            List<NormalizedCourse> source, List<NormalizedCourse> target)
        {
            var matches = new List<CourseMatch>();
            foreach (var sc in source)
            {
                var best = target
                    .Select(tc => new { Course = tc, Score = MatchScore(sc, tc) })
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault();

                if (best != null && best.Score > 0.4)
                    matches.Add(new CourseMatch
                    {
                        SourceCourse = sc.CourseTitle,
                        TargetCourse = best.Course.CourseTitle,
                        OverlapScore = best.Score,
                        DepthDelta = best.Course.DepthRating - sc.DepthRating,
                        BreadthDelta = best.Course.BreadthRating - sc.BreadthRating,
                        MissingTopics = GetMissingTopics(sc, best.Course)
                    });
                else
                    matches.Add(new CourseMatch
                    {
                        SourceCourse = sc.CourseTitle,
                        IsGapCourse = true
                    });
            }
            return matches;
        }

        private static double MatchScore(NormalizedCourse a, NormalizedCourse b) =>
            (TitleSimilarity(a.CourseTitle, b.CourseTitle) * 0.5) +
            (TopicOverlap(a.TopicsJson, b.TopicsJson) * 0.3) +
            (TagOverlap(a.SkillTagsJson, b.SkillTagsJson) * 0.2);

        private static DimensionGaps ComputeDimensionGaps(
            List<NormalizedCourse> source, List<NormalizedCourse> target)
        {
            static double Avg(List<NormalizedCourse> c, Func<NormalizedCourse, int> f) =>
                c.Count == 0 ? 0 : c.Average(f);

            return new DimensionGaps
            {
                CurriculumDepth = Normalize(Avg(target, x => x.DepthRating) - Avg(source, x => x.DepthRating)),
                BreadthCoverage = Normalize(Avg(target, x => x.BreadthRating) - Avg(source, x => x.BreadthRating)),
                PracticalIntensity = Normalize(Avg(target, x => x.PracticalBalance) - Avg(source, x => x.PracticalBalance)),
                LabPresence = Normalize(
                    (double)target.Count(c => c.HasLab) / Math.Max(target.Count, 1) -
                    (double)source.Count(c => c.HasLab) / Math.Max(source.Count, 1)),
                CourseCountGap = target.Count - source.Count
            };
        }

        private static List<TopicGap> FindMissingTopics(
            List<NormalizedCourse> source, List<NormalizedCourse> target)
        {
            var st = source.SelectMany(c => Deserialize(c.TopicsJson)).ToList();
            var tt = target.SelectMany(c => Deserialize(c.TopicsJson)).ToList();
            return tt.Where(t => !st.Any(s => TitleSimilarity(s, t) > 0.7))
                     .Select(t => new TopicGap { Topic = t, Importance = "High" })
                     .Take(20).ToList();
        }

        private static List<CreditMapEntry> MapCredits(
            List<NormalizedCourse> source, List<NormalizedCourse> target)
        {
            return source.Select(sc =>
            {
                var match = target.FirstOrDefault(tc =>
                    TitleSimilarity(sc.CourseTitle, tc.CourseTitle) > 0.6);
                return new CreditMapEntry
                {
                    SourceCourse = sc.CourseTitle,
                    SourceCredits = sc.CreditHours,
                    TargetCourse = match?.CourseTitle,
                    TargetCredits = match?.CreditHours ?? 0,
                    CreditDelta = (match?.CreditHours ?? 0) - sc.CreditHours
                };
            }).ToList();
        }

        private static double ComputeOAGS(DimensionGaps g) =>
            (g.CurriculumDepth * 0.30) + (g.PracticalIntensity * 0.25) +
            (g.BreadthCoverage * 0.20) + (g.LabPresence * 0.15) +
            (Math.Min(g.CourseCountGap * 2.0, 100) * 0.10);

        private static string ClassifySeverity(double s) => s switch
        {
            < 20 => "Minimal",
            < 40 => "Moderate",
            < 65 => "Significant",
            _ => "Critical"
        };

        private static double TitleSimilarity(string a, string b)
        {
            var wa = WordSet(a); var wb = WordSet(b);
            if (wa.Count == 0 || wb.Count == 0) return 0;
            return (double)wa.Intersect(wb).Count() / Math.Max(wa.Count, wb.Count);
        }

        private static double TopicOverlap(string aj, string bj)
        {
            var a = Deserialize(aj); var b = Deserialize(bj);
            if (a.Count == 0 || b.Count == 0) return 0;
            var hits = a.SelectMany(_ => b, (ta, tb) => TitleSimilarity(ta, tb)).Count(s => s > 0.6);
            return (double)hits / Math.Max(a.Count, b.Count);
        }

        private static double TagOverlap(string aj, string bj)
        {
            var a = Deserialize(aj).Select(t => t.ToLower()).ToHashSet();
            var b = Deserialize(bj).Select(t => t.ToLower()).ToHashSet();
            if (a.Count == 0 || b.Count == 0) return 0;
            return (double)a.Intersect(b).Count() / Math.Max(a.Count, b.Count);
        }

        private static List<string> GetMissingTopics(NormalizedCourse s, NormalizedCourse t) =>
            Deserialize(t.TopicsJson)
                .Where(tp => !Deserialize(s.TopicsJson).Any(sp => TitleSimilarity(sp, tp) > 0.6))
                .Take(5).ToList();

        private static List<string> Deserialize(string json)
        {
            try { return JsonSerializer.Deserialize<List<string>>(json) ?? new(); }
            catch { return new(); }
        }

        private static HashSet<string> WordSet(string s) =>
            s.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Where(w => w.Length > 2).ToHashSet();

        private static double Normalize(double raw) =>
            Math.Round(Math.Min(Math.Abs(raw) * 20.0, 100.0), 1);
    }

    // ── Supporting types ──────────────────────────────────────────────────────

    public class UniversityCurriculum
    {
        public string UniversityName { get; set; } = string.Empty;
        public List<NormalizedCourse> Courses { get; set; } = new();
    }

    public class CurriculumComparisonReport
    {
        public string SourceUniversity { get; set; } = string.Empty;
        public string TargetUniversity { get; set; } = string.Empty;
        public double OverallGapScore { get; set; }
        public string Severity { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public DimensionGaps DimensionGaps { get; set; } = new();
        public List<CourseMatch> CourseOverlap { get; set; } = new();
        public List<TopicGap> MissingTopics { get; set; } = new();
        public List<CreditMapEntry> CreditMapping { get; set; } = new();
    }

    public class DimensionGaps
    {
        public double CurriculumDepth { get; set; }
        public double BreadthCoverage { get; set; }
        public double PracticalIntensity { get; set; }
        public double LabPresence { get; set; }
        public int CourseCountGap { get; set; }
    }

    public class CourseMatch
    {
        public string SourceCourse { get; set; } = string.Empty;
        public string? TargetCourse { get; set; }
        public double OverlapScore { get; set; }
        public int DepthDelta { get; set; }
        public int BreadthDelta { get; set; }
        public bool IsGapCourse { get; set; }
        public List<string> MissingTopics { get; set; } = new();
    }

    public class TopicGap
    {
        public string Topic { get; set; } = string.Empty;
        public string Importance { get; set; } = string.Empty;
    }

    public class CreditMapEntry
    {
        public string SourceCourse { get; set; } = string.Empty;
        public int SourceCredits { get; set; }
        public string? TargetCourse { get; set; }
        public int TargetCredits { get; set; }
        public int CreditDelta { get; set; }
    }
}