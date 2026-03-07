using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECM_BE.Data;
using ECM_BE.Models.DTOs.AI;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class AITestRcmService : IAITestRcmService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AITestRcmService(
            AppDbContext context,
            HttpClient httpClient,
            IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<TestRcmDTO?> RecommendTestAsync(string userId)
        {
            Console.WriteLine($"[AITestRcm] Starting test recommendation for user: {userId}");
            
            var userGoal = await _context.UserGoals
                .FirstOrDefaultAsync(x => x.userID == userId);

            if (userGoal == null)
            {
                Console.WriteLine($"[AITestRcm] No user goal found for user: {userId}");
                return null;
            }

            Console.WriteLine($"[AITestRcm] Found user goal: {userGoal.Content}");

            var tests = await _context.PlacementTests
                .Select(t => new
                {
                    t.TestID,
                    t.Title,
                    t.Category,
                    t.Level
                })
                .ToListAsync();

            if (tests.Count == 0)
            {
                Console.WriteLine($"[AITestRcm] No tests available in database");
                return null;
            }

            Console.WriteLine($"[AITestRcm] Found {tests.Count} tests");

            var prompt = $@"
                        Bạn là trợ lý học tập thông minh cho hệ thống ECM.

                        Mục tiêu học tập của người dùng:
                        {userGoal.Content}

                        Danh sách bài đánh giá hiện có:
                        {JsonSerializer.Serialize(tests)}

                        Yêu cầu:
                        - Chọn 1 bài đánh giá phù hợp nhất
                        - Giải thích ngắn gọn lý do
                        - Trả kết quả dưới dạng JSON:
                        {{ ""testId"": number, ""reason"": string }}
                        ";

            Console.WriteLine($"[AITestRcm] Calling OpenAI API...");
            var response = await CallOpenAIAsync(prompt);
            
            if (response == null)
            {
                Console.WriteLine($"[AITestRcm] OpenAI API returned null");
                return null;
            }

            Console.WriteLine($"[AITestRcm] OpenAI response received");

            try
            {
                var result = JsonSerializer.Deserialize<TestRcmDTO>(
                    response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (result != null)
                {
                    Console.WriteLine($"[AITestRcm] Successfully parsed recommendation for test ID: {result.TestId}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AITestRcm] Failed to parse response: {ex.Message}");
                return null;
            }
        }

        private async Task<string?> CallOpenAIAsync(string prompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine($"[AITestRcm] No OpenAI API key configured");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            Console.WriteLine($"[AITestRcm] Using OpenAI model: gpt-4o-mini");

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "Bạn là trợ lý AI học tập. Return ONLY valid JSON, no markdown." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            try
            {
                var res = await _httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    content);

                if (!res.IsSuccessStatusCode)
                {
                    var errorContent = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AITestRcm] OpenAI API error: {res.StatusCode} - {errorContent}");
                    return null;
                }

                var json = await res.Content.ReadAsStringAsync();
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
                Console.WriteLine($"[AITestRcm] Exception calling OpenAI: {ex.Message}");
                return null;
            }
        }
    }
}
