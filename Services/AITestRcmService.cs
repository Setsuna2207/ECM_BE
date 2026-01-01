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
            var userGoal = await _context.UserGoals
                .FirstOrDefaultAsync(x => x.userID == userId);

            if (userGoal == null) return null;

            var tests = await _context.PlacementTests
                .Select(t => new
                {
                    t.TestID,
                    t.Title,
                    t.Category,
                    t.Level
                })
                .ToListAsync();

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

            var response = await CallOpenAIAsync(prompt);
            if (response == null) return null;

            return JsonSerializer.Deserialize<TestRcmDTO>(
                response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private async Task<string?> CallOpenAIAsync(string prompt)
        {
            var apiKey = _config["OpenAI:ApiKey"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                    new { role = "system", content = "Bạn là trợ lý AI học tập." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.3
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var res = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                content);

            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
