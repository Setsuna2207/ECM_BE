using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECM_BE.Data;
using ECM_BE.Models.DTOs.AI;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class AICourseRcmService : IAICourseRcmService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        private const string SystemPrompt = @"
                You are an AI assistant for an e-learning platform.
                You must recommend courses based ONLY on the provided data.
                You must NOT invent courses.
                You must return VALID JSON ONLY.
                Do not include any text outside JSON.
                ";

        public AICourseRcmService(
            AppDbContext context,
            HttpClient httpClient,
            IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<CourseRcmDTO?> RecommendCourseAsync(string userId)
        {
            // First, try to get saved recommendations from database
            var savedRcm = await _context.AIRcms
                .Where(x => x.userID == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (savedRcm != null)
            {
                try
                {
                    var recommendations = JsonSerializer.Deserialize<List<CourseRcmItemDTO>>(
                        savedRcm.RcmCourses,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (recommendations != null && recommendations.Any())
                    {
                        Console.WriteLine($"[AICourseRcm] ✅ Retrieved saved recommendations for user {userId}");
                        return new CourseRcmDTO { Recommendations = recommendations };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AICourseRcm] ⚠️ Error parsing saved recommendations: {ex.Message}");
                }
            }

            // If no saved recommendations, generate new ones
            Console.WriteLine($"[AICourseRcm] Generating new recommendations for user {userId}");

            // User goal
            var userGoal = await _context.UserGoals
                .FirstOrDefaultAsync(x => x.userID == userId);

            if (userGoal == null)
                return null;

            // Test result
            var testResult = await _context.TestResults
                .Where(x => x.userID == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (testResult == null)
                return null;

            // Parse section scores
            Dictionary<string, float>? sectionScores = null;

            if (!string.IsNullOrEmpty(testResult.SectionScores))
            {
                try
                {
                    sectionScores = JsonSerializer.Deserialize<Dictionary<string, float>>(
                        testResult.SectionScores,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                }
                catch
                {
                    sectionScores = null;
                }
            }

            // Courses data
            var courses = await _context.Courses
                .Include(c => c.Categories)
                .Select(c => new
                {
                    courseId = c.CourseID,
                    title = c.Title,
                    categories = c.Categories.Select(cat => new
                    {
                        name = cat.Name,
                        type = cat.Description // SKILL / LEVEL
                    })
                })
                .ToListAsync();

            if (!courses.Any())
                return null;

            // Build user prompt
            var skillScoreText = sectionScores == null
                ? "No detailed skill scores available."
                : string.Join(
                    "\n",
                    sectionScores.Select(s => $"  - {s.Key}: {s.Value}")
                  );

            var userPrompt = $@"
                    User learning goal:
                    ""{userGoal.Content}""

                    Placement test result:
                    - Overall score: {testResult.OverallScore}
                    - Detected level: {testResult.LevelDetected ?? "Unknown"}
                    - Skill scores:
                    {skillScoreText}

                    Available courses:
                    {JsonSerializer.Serialize(courses)}

                    Rules:
                    1. Only recommend courses from the provided list.
                    2. Prioritize courses that address weak skills.
                    3. Recommend at most 3 courses.

                    Return JSON in this format:
                    {{
                      ""recommendations"": [
                        {{
                          ""courseId"": number,
                          ""reason"": string
                        }}
                      ]
                    }}
                    ";

            // OpenAI API call
            var aiRawResponse = await CallOpenAIAsync(SystemPrompt, userPrompt);
            if (aiRawResponse == null)
                return null;

            // Parse AI response
            CourseRcmDTO? aiResult;
            try
            {
                aiResult = JsonSerializer.Deserialize<CourseRcmDTO>(
                    aiRawResponse,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                return null;
            }

            if (aiResult == null || aiResult.Recommendations == null)
                return null;

            // Filter courseIds
            var validCourseIds = courses
                .Select(c => c.courseId)
                .ToHashSet();

            aiResult.Recommendations = aiResult.Recommendations
                .Where(r => validCourseIds.Contains(r.CourseId))
                .Take(3)
                .ToList();

            // Save recommendations to database
            try
            {
                var rcmJson = JsonSerializer.Serialize(aiResult.Recommendations);
                
                var aiRcm = new AIRcm
                {
                    userID = userId,
                    RcmCourses = rcmJson,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AIRcms.Add(aiRcm);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[AICourseRcm] ✅ Saved recommendations for user {userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AICourseRcm] ⚠️ Warning: Could not save recommendations: {ex.Message}");
            }

            return aiResult;
        }

        private async Task<string?> CallOpenAIAsync(
            string systemPrompt,
            string userPrompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.2
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                content);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
