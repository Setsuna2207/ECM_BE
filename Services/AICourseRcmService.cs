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
                You are an AI assistant for an e-learning platform specializing in English language test preparation (TOEFL, IELTS, TOEIC).
                Your task is to recommend courses that EXACTLY match the user's learning goal.
                You must recommend courses based ONLY on the provided data.
                You must NOT invent courses.
                You must return VALID JSON ONLY.
                Do not include any text outside JSON.
                Pay close attention to the specific exam type (TOEFL/IELTS/TOEIC/GENERAL) mentioned in the user

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
            Console.WriteLine($"[AICourseRcm] Starting course recommendation for user: {userId}");
            
            // First, try to get saved recommendations from database
            var savedRcm = await _context.AIRcms
                .Where(x => x.userID == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (savedRcm != null)
            {
                Console.WriteLine($"[AICourseRcm] Found cached recommendation from {savedRcm.CreatedAt}");
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
                        Console.WriteLine($"[AICourseRcm] Returning {recommendations.Count} cached recommendations");
                        return new CourseRcmDTO { Recommendations = recommendations };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AICourseRcm] Error parsing cached recommendations: {ex.Message}");
                    // Error parsing saved recommendations, continue to generate new ones
                }
            }

            Console.WriteLine($"[AICourseRcm] No valid cache found, generating new recommendations");

            // If no saved recommendations, generate new ones

            // User goal
            var userGoal = await _context.UserGoals
                .FirstOrDefaultAsync(x => x.userID == userId);

            if (userGoal == null)
            {
                Console.WriteLine($"[AICourseRcm] No user goal found for user: {userId}");
                return null;
            }

            Console.WriteLine($"[AICourseRcm] Found user goal: {userGoal.Content}");

            // Test result
            var testResult = await _context.TestResults
                .Where(x => x.userID == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (testResult == null)
            {
                Console.WriteLine($"[AICourseRcm] No test result found for user: {userId}");
                return null;
            }

            Console.WriteLine($"[AICourseRcm] Found test result - Score: {testResult.OverallScore}, Level: {testResult.LevelDetected}");

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
            {
                Console.WriteLine($"[AICourseRcm] No courses found in database");
                return null;
            }

            Console.WriteLine($"[AICourseRcm] Found {courses.Count} courses in database");

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
                    1. CRITICAL: Only recommend courses that are DIRECTLY RELEVANT to the user's learning goal ""{userGoal.Content}"".
                    2. If the goal mentions TOEFL, only recommend TOEFL courses. If it mentions IELTS, only IELTS courses. If it mentions TOEIC, only TOEIC courses. If it mentions GENERAL, only GENERAL courses
                    3. Match the course content and categories to the specific exam/skill mentioned in the goal.
                    4. Prioritize courses that address weak skills shown in the test result.
                    5. Recommend at most 3 courses.
                    6. If no courses match the user's goal, return an empty recommendations array.

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
            Console.WriteLine($"[AICourseRcm] Calling OpenAI API...");
            var aiRawResponse = await CallOpenAIAsync(SystemPrompt, userPrompt);
            if (aiRawResponse == null)
            {
                Console.WriteLine($"[AICourseRcm] OpenAI API returned null");
                return null;
            }

            Console.WriteLine($"[AICourseRcm] OpenAI response received");

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
            catch (Exception ex)
            {
                Console.WriteLine($"[AICourseRcm] Failed to parse AI response: {ex.Message}");
                return null;
            }

            if (aiResult == null || aiResult.Recommendations == null)
            {
                Console.WriteLine($"[AICourseRcm] AI result is null or has no recommendations");
                return null;
            }

            Console.WriteLine($"[AICourseRcm] Parsed {aiResult.Recommendations.Count} recommendations from AI");

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
                Console.WriteLine($"[AICourseRcm] Successfully saved {aiResult.Recommendations.Count} recommendations to database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AICourseRcm] Failed to save recommendations: {ex.Message}");
                // Failed to save recommendations, but don't block the response
            }

            Console.WriteLine($"[AICourseRcm] Returning {aiResult.Recommendations.Count} recommendations");
            return aiResult;
        }

        private async Task<string?> CallOpenAIAsync(
            string systemPrompt,
            string userPrompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine($"[AICourseRcm] No OpenAI API key configured");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            Console.WriteLine($"[AICourseRcm] Using OpenAI model: gpt-4o-mini");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt + " Return ONLY valid JSON, no markdown." },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.2
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AICourseRcm] OpenAI API error: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var responseText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                // Clean up markdown code blocks if present
                if (responseText != null)
                {
                    responseText = responseText.Trim();
                    if (responseText.StartsWith("```json"))
                    {
                        responseText = responseText.Substring(7);
                    }
                    if (responseText.StartsWith("```"))
                    {
                        responseText = responseText.Substring(3);
                    }
                    if (responseText.EndsWith("```"))
                    {
                        responseText = responseText.Substring(0, responseText.Length - 3);
                    }
                    responseText = responseText.Trim();
                }

                return responseText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AICourseRcm] Exception calling OpenAI: {ex.Message}");
                return null;
            }
        }
    }
}
