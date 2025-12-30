using ECM_BE.Data;
using ECM_BE.Models.DTOs.AIRecommendation;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ECM_BE.Services
{
    public class AIRecommendationService : IAIRecommendationService
    {
        private readonly AppDbContext _context;

        public AIRecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GoalAnalysisResponseDTO> AnalyzeGoalAndRecommendTestAsync(GoalAnalysisRequestDTO request)
        {
            // Get the UserGoal from database
            var userGoal = await _context.UserGoals
                .Where(g => g.userID == request.UserID)
                .OrderByDescending(g => g.UpdatedAt)
                .FirstOrDefaultAsync();

            if (userGoal == null)
            {
                throw new Exception("Không tìm thấy mục tiêu học tập. Vui lòng thiết lập mục tiêu trước.");
            }

            // Parse the goal using AI-like logic (content-based filtering)
            var parsedGoal = ParseUserGoal(userGoal.Content);

            // Find appropriate placement test based on parsed goal
            var recommendedTest = await FindBestPlacementTest(parsedGoal);

            if (recommendedTest == null)
            {
                throw new Exception("Không tìm thấy bài kiểm tra phù hợp với mục tiêu của bạn");
            }

            // Create or update LearningPath
            var learningPath = await _context.LearningPaths
                .FirstOrDefaultAsync(lp => lp.UserGoalID == userGoal.UserGoalID && lp.Status != "Completed");

            if (learningPath == null)
            {
                learningPath = new LearningPath
                {
                    userID = request.UserID!,
                    UserGoalID = userGoal.UserGoalID,
                    InitialTestID = recommendedTest.TestID,
                    Status = "TestRecommended",
                    CreatedAt = DateTime.UtcNow
                };
                _context.LearningPaths.Add(learningPath);
            }
            else
            {
                learningPath.InitialTestID = recommendedTest.TestID;
                learningPath.Status = "TestRecommended";
                learningPath.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new GoalAnalysisResponseDTO
            {
                LearningPathID = learningPath.LearningPathID,
                ParsedGoal = userGoal.Content,
                Category = parsedGoal.Category,
                Skill = parsedGoal.Skill,
                TargetScore = parsedGoal.TargetScore,
                Level = parsedGoal.Level,
                RecommendedTestID = recommendedTest.TestID,
                RecommendedTestTitle = recommendedTest.Title,
                Message = $"Dựa trên mục tiêu '{userGoal.Content}', chúng tôi khuyên bạn nên làm bài kiểm tra '{recommendedTest.Title}' để đánh giá trình độ hiện tại."
            };
        }

        public async Task<CourseRecommendationResponseDTO> RecommendCoursesBasedOnResultAsync(CourseRecommendationRequestDTO request)
        {
            // Get learning path
            var learningPath = await _context.LearningPaths
                .Include(lp => lp.UserGoal)
                .FirstOrDefaultAsync(lp => lp.LearningPathID == request.LearningPathID);

            if (learningPath == null)
            {
                throw new Exception("Không tìm thấy lộ trình học tập");
            }

            // Get test result
            var testResult = await _context.TestResults
                .Include(tr => tr.PlacementTest)
                .FirstOrDefaultAsync(tr => tr.ResultID == request.ResultID);

            if (testResult == null)
            {
                throw new Exception("Không tìm thấy kết quả kiểm tra");
            }

            // Analyze weak skills from test result
            var weakSkills = AnalyzeWeakSkills(testResult);

            // Get goal information
            var goalContent = learningPath.UserGoal.Content;
            var parsedGoal = ParseUserGoal(goalContent);

            // Find courses that match weak skills and goal
            var recommendedCourses = await FindRecommendedCourses(parsedGoal, weakSkills, testResult);

            // Create AI Feedback
            var aiFeedback = new AIFeedback
            {
                ResultID = testResult.ResultID,
                WeakSkill = string.Join(", ", weakSkills),
                RcmCourses = JsonConvert.SerializeObject(recommendedCourses.Select(c => c.CourseID).ToList()),
                FeedbackSummary = GenerateFeedbackSummary(testResult, weakSkills, parsedGoal),
                CreatedAt = DateTime.UtcNow
            };
            _context.AIFeedbacks.Add(aiFeedback);

            // Update learning path
            learningPath.InitialResultID = testResult.ResultID;
            learningPath.RecommendedCourses = JsonConvert.SerializeObject(recommendedCourses.Select(c => c.CourseID).ToList());
            learningPath.Status = "CoursesRecommended";
            learningPath.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CourseRecommendationResponseDTO
            {
                LearningPathID = learningPath.LearningPathID,
                RecommendedCourses = recommendedCourses,
                WeakSkills = string.Join(", ", weakSkills),
                FeedbackSummary = aiFeedback.FeedbackSummary,
                Message = "Dựa trên kết quả kiểm tra của bạn, chúng tôi đề xuất các khóa học sau để giúp bạn đạt mục tiêu."
            };
        }

        public async Task<GoalAnalysisResponseDTO> GenerateFinalAssessmentAsync(int learningPathId)
        {
            var learningPath = await _context.LearningPaths
                .Include(lp => lp.UserGoal)
                .Include(lp => lp.InitialTest)
                .FirstOrDefaultAsync(lp => lp.LearningPathID == learningPathId);

            if (learningPath == null)
            {
                throw new Exception("Không tìm thấy lộ trình học tập");
            }

            // Check if all recommended courses are completed
            var recommendedCourseIds = JsonConvert.DeserializeObject<List<int>>(learningPath.RecommendedCourses ?? "[]");
            var completedCourseIds = JsonConvert.DeserializeObject<List<int>>(learningPath.CompletedCourses ?? "[]");

            if (recommendedCourseIds == null || completedCourseIds == null || 
                !recommendedCourseIds.All(id => completedCourseIds.Contains(id)))
            {
                throw new Exception("Bạn cần hoàn thành tất cả các khóa học được đề xuất trước khi làm bài kiểm tra cuối cùng");
            }

            // Use the same test as initial test for final assessment
            var finalTest = learningPath.InitialTest;

            if (finalTest == null)
            {
                throw new Exception("Không tìm thấy bài kiểm tra phù hợp");
            }

            // Update learning path
            learningPath.FinalTestID = finalTest.TestID;
            learningPath.Status = "ReadyForFinal";
            learningPath.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var parsedGoal = ParseUserGoal(learningPath.UserGoal.Content);

            return new GoalAnalysisResponseDTO
            {
                LearningPathID = learningPath.LearningPathID,
                ParsedGoal = learningPath.UserGoal.Content,
                Category = parsedGoal.Category,
                Skill = parsedGoal.Skill,
                TargetScore = parsedGoal.TargetScore,
                Level = parsedGoal.Level,
                RecommendedTestID = finalTest.TestID,
                RecommendedTestTitle = finalTest.Title,
                Message = "Chúc mừng! Bạn đã hoàn thành tất cả các khóa học. Hãy làm bài kiểm tra cuối cùng để xem bạn đã đạt mục tiêu chưa."
            };
        }

        public async Task<bool> UpdateCourseCompletionAsync(int learningPathId, int courseId)
        {
            var learningPath = await _context.LearningPaths
                .FirstOrDefaultAsync(lp => lp.LearningPathID == learningPathId);

            if (learningPath == null)
            {
                return false;
            }

            var completedCourses = JsonConvert.DeserializeObject<List<int>>(learningPath.CompletedCourses ?? "[]") ?? new List<int>();

            if (!completedCourses.Contains(courseId))
            {
                completedCourses.Add(courseId);
                learningPath.CompletedCourses = JsonConvert.SerializeObject(completedCourses);
                learningPath.Status = "InProgress";
                learningPath.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return true;
        }

        // Helper Methods

        private ParsedGoal ParseUserGoal(string goalContent)
        {
            var goal = goalContent.ToLower();
            var parsed = new ParsedGoal();

            // Extract category
            if (goal.Contains("toeic")) parsed.Category = "TOEIC";
            else if (goal.Contains("ielts")) parsed.Category = "IELTS";
            else if (goal.Contains("toefl")) parsed.Category = "TOEFL";
            else parsed.Category = "GENERAL";

            // Extract skill
            if (goal.Contains("reading") || goal.Contains("đọc")) parsed.Skill = "READING";
            else if (goal.Contains("listening") || goal.Contains("nghe")) parsed.Skill = "LISTENING";
            else if (goal.Contains("writing") || goal.Contains("viết")) parsed.Skill = "WRITING";
            else if (goal.Contains("speaking") || goal.Contains("nói")) parsed.Skill = "SPEAKING";
            else parsed.Skill = "ALL";

            // Extract target score
            var scoreMatch = Regex.Match(goal, @"\b(\d{3,4})\b");
            if (scoreMatch.Success)
            {
                parsed.TargetScore = int.Parse(scoreMatch.Value);
            }

            // Determine level based on target score
            if (parsed.TargetScore.HasValue)
            {
                if (parsed.TargetScore < 400) parsed.Level = "Beginner";
                else if (parsed.TargetScore < 700) parsed.Level = "Intermediate";
                else parsed.Level = "Advanced";
            }
            else
            {
                parsed.Level = "Intermediate"; // Default
            }

            return parsed;
        }

        private async Task<PlacementTest?> FindBestPlacementTest(ParsedGoal parsedGoal)
        {
            var query = _context.PlacementTests.AsQueryable();

            // Filter by category
            if (parsedGoal.Category != "GENERAL")
            {
                query = query.Where(t => t.Category != null && t.Category.ToUpper() == parsedGoal.Category);
            }

            // Filter by level
            query = query.Where(t => t.Level == null || t.Level == "All Levels" || t.Level == parsedGoal.Level);

            // Get the first matching test
            var test = await query.FirstOrDefaultAsync();

            // If no specific test found, get any general test
            if (test == null)
            {
                test = await _context.PlacementTests.FirstOrDefaultAsync();
            }

            return test;
        }

        private List<string> AnalyzeWeakSkills(TestResult testResult)
        {
            var weakSkills = new List<string>();

            // Parse section scores
            if (!string.IsNullOrEmpty(testResult.SectionScores))
            {
                try
                {
                    var sectionScores = JsonConvert.DeserializeObject<Dictionary<string, float>>(testResult.SectionScores);

                    if (sectionScores != null)
                    {
                        foreach (var section in sectionScores)
                        {
                            // If score is below 60%, consider it a weak skill
                            if (section.Value < 60)
                            {
                                weakSkills.Add(section.Key);
                            }
                        }
                    }
                }
                catch
                {
                    // If parsing fails, analyze overall score
                }
            }

            // If no section scores or all sections are good, analyze overall performance
            if (weakSkills.Count == 0)
            {
                var percentage = (testResult.CorrectAnswers / (float)(testResult.CorrectAnswers + testResult.IncorrectAnswers + testResult.SkippedAnswers)) * 100;

                if (percentage < 60)
                {
                    weakSkills.Add("General Skills");
                }
            }

            return weakSkills;
        }

        private async Task<List<RecommendedCourseDTO>> FindRecommendedCourses(ParsedGoal parsedGoal, List<string> weakSkills, TestResult testResult)
        {
            var courses = await _context.Courses
                .Include(c => c.Categories)
                .ToListAsync();

            var recommendedCourses = new List<RecommendedCourseDTO>();
            int priority = 1;

            foreach (var course in courses)
            {
                var matchScore = 0;
                var reasons = new List<string>();

                // Check if course matches the goal category
                var courseCategories = course.Categories.Select(c => c.Name.ToLower()).ToList();

                if (courseCategories.Any(cat => cat.Contains(parsedGoal.Category.ToLower())))
                {
                    matchScore += 3;
                    reasons.Add($"Phù hợp với mục tiêu {parsedGoal.Category}");
                }

                // Check if course addresses weak skills
                foreach (var weakSkill in weakSkills)
                {
                    if (courseCategories.Any(cat => cat.Contains(weakSkill.ToLower())))
                    {
                        matchScore += 2;
                        reasons.Add($"Cải thiện kỹ năng yếu: {weakSkill}");
                    }
                }

                // Check if course matches the skill focus
                if (parsedGoal.Skill != "ALL" && courseCategories.Any(cat => cat.Contains(parsedGoal.Skill.ToLower())))
                {
                    matchScore += 2;
                    reasons.Add($"Tập trung vào {parsedGoal.Skill}");
                }

                if (matchScore > 0)
                {
                    recommendedCourses.Add(new RecommendedCourseDTO
                    {
                        CourseID = course.CourseID,
                        Title = course.Title ?? "Untitled Course",
                        Description = course.Description,
                        ThumbnailUrl = course.ThumbnailUrl,
                        Reason = string.Join(". ", reasons),
                        Priority = priority++
                    });
                }
            }

            // Sort by match score (implicit in priority) and return top 6
            return recommendedCourses.OrderBy(c => c.Priority).Take(6).ToList();
        }

        private string GenerateFeedbackSummary(TestResult testResult, List<string> weakSkills, ParsedGoal parsedGoal)
        {
            var percentage = (testResult.CorrectAnswers / (float)(testResult.CorrectAnswers + testResult.IncorrectAnswers + testResult.SkippedAnswers)) * 100;

            var summary = $"Bạn đã hoàn thành bài kiểm tra với {percentage:F1}% câu trả lời đúng. ";

            if (weakSkills.Count > 0)
            {
                summary += $"Các kỹ năng cần cải thiện: {string.Join(", ", weakSkills)}. ";
            }

            summary += $"Để đạt mục tiêu '{parsedGoal.Category} {parsedGoal.Skill}";

            if (parsedGoal.TargetScore.HasValue)
            {
                summary += $" {parsedGoal.TargetScore}";
            }

            summary += "', chúng tôi đề xuất bạn học các khóa học được gợi ý bên dưới.";

            return summary;
        }

        private class ParsedGoal
        {
            public string Category { get; set; } = "GENERAL";
            public string Skill { get; set; } = "ALL";
            public int? TargetScore { get; set; }
            public string Level { get; set; } = "Intermediate";
        }
    }
}
