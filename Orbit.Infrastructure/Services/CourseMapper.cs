using Orbit.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Orbit.Infrastructure.Pdf;
using System.Text.Json;

namespace Orbit.Infrastructure.Services
{
    public static class CourseMapper
    {
        public static NormalizedCourse ToNormalized(
            StructuredCourseBlock block, int universityId)
        {
            return new NormalizedCourse
            {
                UniversityId = universityId,
                CourseTitle = block.CourseTitle,
                CourseCode = block.CourseCode,
                Semester = block.Semester,
                CreditHours = block.CreditHours,
                TheoryHours = block.TheoryHours,
                LabHours = block.LabHours,
                HasLab = block.HasLab,
                TopicsJson = Serialize(block.Topics),
                PrerequisitesJson = Serialize(block.Prerequisites),
                LearningOutcomesJson = Serialize(block.LearningOutcomes),
                SkillTagsJson = Serialize(block.SkillTags),
                DepthRating = block.DepthRating,
                BreadthRating = block.BreadthRating,
                PracticalBalance = block.PracticalBalance,
                AssessmentStyle = block.AssessmentStyle,
                DifficultyLevel = block.DifficultyLevel,
                SourceFormat = block.SourceFormat.ToString(),
                IsAdminVerified = false
            };
        }

        private static string Serialize(List<string> list) =>
            JsonSerializer.Serialize(list);
    }
}
