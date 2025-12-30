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

            // Mark ALL old learning paths as archived when new goal is set
            // This ensures only the newest learning path is active
            var oldLearningPaths = await _context.LearningPaths
                .Where(lp => lp.userID == request.UserID && lp.Status != "Archived")
                .ToListAsync();

            foreach (var oldPath in oldLearningPaths)
            {
                oldPath.Status = "Archived";
                oldPath.UpdatedAt = DateTime.UtcNow;
            }

            Console.WriteLine($"[AnalyzeGoal] Archived {oldLearningPaths.Count} old learning paths for user {request.UserID}");

            // Create NEW learning path for the new goal
            var learningPath = new LearningPath
            {
                userID = request.UserID!,
                UserGoalID = userGoal.UserGoalID,
                InitialTestID = recommendedTest.TestID,
                Status = "TestRecommended",
                CreatedAt = DateTime.UtcNow
            };
            _context.LearningPaths.Add(learningPath);

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
            Console.WriteLine($"[RecommendCourses] Learning Path ID: {request.LearningPathID}");
            Console.WriteLine($"[RecommendCourses] Result ID: {request.ResultID}");

            // Get learning path
            var learningPath = await _context.LearningPaths
                .Include(lp => lp.UserGoal)
                .FirstOrDefaultAsync(lp => lp.LearningPathID == request.LearningPathID);

            if (learningPath == null)
            {
                throw new Exception("Không tìm thấy lộ trình học tập");
            }

            Console.WriteLine($"[RecommendCourses] Learning Path Status: {learningPath.Status}");
            Console.WriteLine($"[RecommendCourses] Goal: {learningPath.UserGoal?.Content}");

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
            Console.WriteLine($"[RecommendCourses] Weak Skills: {string.Join(", ", weakSkills)}");

            // Get goal information
            var goalContent = learningPath.UserGoal.Content;
            var parsedGoal = ParseUserGoal(goalContent);
            Console.WriteLine($"[RecommendCourses] Parsed Goal - Category: {parsedGoal.Category}, Skill: {parsedGoal.Skill}");

            // Find courses that match weak skills and goal
            var recommendedCourses = await FindRecommendedCourses(parsedGoal, weakSkills, testResult);
            Console.WriteLine($"[RecommendCourses] Found {recommendedCourses.Count} recommended courses");

            // Create or update AI Feedback
            var existingFeedback = await _context.AIFeedbacks
                .FirstOrDefaultAsync(f => f.ResultID == testResult.ResultID);

            if (existingFeedback != null)
            {
                // Update existing feedback
                existingFeedback.WeakSkill = string.Join(", ", weakSkills);
                existingFeedback.RcmCourses = JsonConvert.SerializeObject(recommendedCourses.Select(c => c.CourseID).ToList());
                existingFeedback.FeedbackSummary = GenerateFeedbackSummary(testResult, weakSkills, parsedGoal);
                Console.WriteLine($"[RecommendCourses] Updated existing AI feedback for result {testResult.ResultID}");
            }
            else
            {
                // Create new feedback
                var aiFeedback = new AIFeedback
                {
                    ResultID = testResult.ResultID,
                    WeakSkill = string.Join(", ", weakSkills),
                    RcmCourses = JsonConvert.SerializeObject(recommendedCourses.Select(c => c.CourseID).ToList()),
                    FeedbackSummary = GenerateFeedbackSummary(testResult, weakSkills, parsedGoal),
                    CreatedAt = DateTime.UtcNow
                };
                _context.AIFeedbacks.Add(aiFeedback);
                Console.WriteLine($"[RecommendCourses] Created new AI feedback for result {testResult.ResultID}");
            }

            // Update learning path
            learningPath.InitialResultID = testResult.ResultID;
            learningPath.RecommendedCourses = JsonConvert.SerializeObject(recommendedCourses.Select(c => new
            {
                courseID = c.CourseID,
                reason = c.Reason,
                priority = c.Priority
            }).ToList());
            learningPath.Status = "CoursesRecommended";
            learningPath.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            Console.WriteLine($"[RecommendCourses] Updated learning path {learningPath.LearningPathID} with recommendations");

            // Get the feedback summary for response
            var feedbackSummary = existingFeedback?.FeedbackSummary ?? GenerateFeedbackSummary(testResult, weakSkills, parsedGoal);

            return new CourseRecommendationResponseDTO
            {
                LearningPathID = learningPath.LearningPathID,
                RecommendedCourses = recommendedCourses,
                WeakSkills = string.Join(", ", weakSkills),
                FeedbackSummary = feedbackSummary,
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

            Console.WriteLine($"[FindRecommendedCourses] Total courses in database: {courses.Count}");
            Console.WriteLine($"[FindRecommendedCourses] Looking for Category: {parsedGoal.Category}, Skill: {parsedGoal.Skill}");

            var recommendedCourses = new List<RecommendedCourseDTO>();
            int priority = 1;

            foreach (var course in courses)
            {
                var matchScore = 0;
                var reasons = new List<string>();

                // Get course categories as lowercase strings
                var courseCategories = course.Categories.Select(c => c.Name.ToLower()).ToList();
                Console.WriteLine($"[FindRecommendedCourses] Course {course.CourseID} '{course.Title}' has categories: {string.Join(", ", courseCategories)}");

                // STRICT MATCHING: Course MUST match BOTH category AND skill
                bool matchesCategory = courseCategories.Any(cat => cat.Contains(parsedGoal.Category.ToLower()));
                bool matchesSkill = parsedGoal.Skill == "ALL" || courseCategories.Any(cat => cat.Contains(parsedGoal.Skill.ToLower()));

                Console.WriteLine($"[FindRecommendedCourses] Course {course.CourseID}: matchesCategory={matchesCategory}, matchesSkill={matchesSkill}");

                // Skip course if it doesn't match BOTH category AND skill
                if (!matchesCategory || !matchesSkill)
                {
                    continue;
                }

                // Course matches both category and skill - add base score
                matchScore += 5;
                reasons.Add($"Phù hợp với mục tiêu {parsedGoal.Category} {parsedGoal.Skill}");

                // Bonus points if course addresses weak skills
                foreach (var weakSkill in weakSkills)
                {
                    if (courseCategories.Any(cat => cat.Contains(weakSkill.ToLower())))
                    {
                        matchScore += 2;
                        reasons.Add($"Cải thiện kỹ năng yếu: {weakSkill}");
                        break; // Only add bonus once per course
                    }
                }

                // Add course to recommendations
                recommendedCourses.Add(new RecommendedCourseDTO
                {
                    CourseID = course.CourseID,
                    Title = course.Title ?? "Untitled Course",
                    Description = course.Description,
                    ThumbnailUrl = course.ThumbnailUrl,
                    Reason = string.Join(". ", reasons),
                    Priority = priority++
                });

                Console.WriteLine($"[FindRecommendedCourses] ✅ Course {course.CourseID} ADDED to recommendations");
            }

            Console.WriteLine($"[FindRecommendedCourses] Total recommended courses: {recommendedCourses.Count}");

            // Sort by match score (higher score = higher priority) and return top 6
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
