using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECM_BE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly IAITestRcmService _testService;
        private readonly IAICourseRcmService _courseService;
        private readonly Data.AppDbContext _context;

        public AIController(
            IAITestRcmService testService,
            IAICourseRcmService courseService,
            Data.AppDbContext context)
        {
            _testService = testService;
            _courseService = courseService;
            _context = context;
        }

        [HttpGet("recommend-test")]
        public async Task<IActionResult> RecommendTest()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                Console.WriteLine($"[AIController] RecommendTest endpoint hit");
                Console.WriteLine($"[AIController] User authenticated: {User.Identity?.IsAuthenticated}");
                Console.WriteLine($"[AIController] UserId: {userId}");
                
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine($"[AIController] No userId, returning Unauthorized");
                    return Unauthorized(new { message = "Bạn cần đăng nhập để sử dụng tính năng này" });
                }
                
                var result = await _testService.RecommendTestAsync(userId);
                
                if (result == null)
                {
                    Console.WriteLine($"[AIController] No test recommendation found");
                    return NotFound(new { message = "Bạn cần thiết lập mục tiêu học tập trước khi nhận gợi ý bài test" });
                }
                
                Console.WriteLine($"[AIController] Returning test recommendation: TestId={result.TestId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIController] Exception in RecommendTest: {ex.Message}");
                Console.WriteLine($"[AIController] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo gợi ý bài test", error = ex.Message });
            }
        }

        [HttpGet("recommend-course")]
        public async Task<IActionResult> RecommendCourse()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                Console.WriteLine($"[AIController] RecommendCourse endpoint hit");
                Console.WriteLine($"[AIController] User authenticated: {User.Identity?.IsAuthenticated}");
                Console.WriteLine($"[AIController] UserId: {userId}");
                
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine($"[AIController] No userId, returning Unauthorized");
                    return Unauthorized(new { message = "Bạn cần đăng nhập để sử dụng tính năng này" });
                }
                
                var result = await _courseService.RecommendCourseAsync(userId);
                
                if (result == null)
                {
                    Console.WriteLine($"[AIController] No course recommendation found");
                    return NotFound(new { message = "Bạn cần thiết lập mục tiêu học tập và hoàn thành bài kiểm tra đầu vào trước khi nhận gợi ý khóa học" });
                }
                
                Console.WriteLine($"[AIController] Returning {result.Recommendations.Count} course recommendations");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIController] Exception in RecommendCourse: {ex.Message}");
                Console.WriteLine($"[AIController] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo gợi ý khóa học", error = ex.Message });
            }
        }

        [HttpGet("recommend-course/{userId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> RecommendCourseForUser(string userId)
        {
            var result = await _courseService.RecommendCourseAsync(userId);
            
            if (result == null)
            {
                return NotFound(new { message = "Người dùng chưa có mục tiêu học tập hoặc chưa hoàn thành bài kiểm tra đầu vào" });
            }
            
            return Ok(result);
        }

        [HttpDelete("clear-cache")]
        public async Task<IActionResult> ClearRecommendationCache()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Bạn cần đăng nhập để sử dụng tính năng này" });
                }

                var cachedRecommendations = await _context.AIRcms
                    .Where(x => x.userID == userId)
                    .ToListAsync();
                
                if (cachedRecommendations.Any())
                {
                    _context.AIRcms.RemoveRange(cachedRecommendations);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[AIController] Cleared {cachedRecommendations.Count} cached recommendations for user {userId}");
                    return Ok(new { message = $"Đã xóa {cachedRecommendations.Count} gợi ý đã lưu", count = cachedRecommendations.Count });
                }
                
                return Ok(new { message = "Không có gợi ý nào được lưu", count = 0 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AIController] Exception in ClearCache: {ex.Message}");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa cache", error = ex.Message });
            }
        }
    }
}
