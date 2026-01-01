using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECM_BE.Data;
using ECM_BE.Models.DTOs.AI;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class AICourseRcmService : IAICourseRcmService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

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
            var userGoal = await _context.UserGoals
                .FirstOrDefaultAsync(x => x.userID == userId);

            var testResult = await _context.TestResults
                .Where(x => x.userID == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (userGoal == null || testResult == null) return null;

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

            var prompt = $@"
                        Bạn là trợ lý học tập thông minh cho hệ thống ECM.

                        Mục tiêu học tập:
                        {userGoal.Content}

                        Kết quả bài đánh giá:
                        - Điểm tổng: {testResult.OverallScore}
                        - Điểm theo kỹ năng:
                        {testResult.SectionScores}

                        Danh sách khóa học hiện có:
                        {JsonSerializer.Serialize(courses)}

                        Yêu cầu:
                        - Gợi ý tối đa 3 khóa học phù hợp
                        - Ưu tiên kỹ năng yếu
                        - Giải thích rõ lý do
                        - Trả về JSON theo format:
                        {{ ""recommendations"": [{{ ""courseId"": number, ""reason"": string }}] }}
                        ";

            var response = await CallOpenAIAsync(prompt);
            if (response == null) return null;

            return JsonSerializer.Deserialize<CourseRcmDTO>(
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
