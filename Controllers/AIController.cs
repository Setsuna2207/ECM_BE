using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AIController(
            IAITestRcmService testService,
            IAICourseRcmService courseService)
        {
            _testService = testService;
            _courseService = courseService;
        }

        [HttpGet("recommend-test")]
        public async Task<IActionResult> RecommendTest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _testService.RecommendTestAsync(userId!);
            return Ok(result);
        }

        [HttpGet("recommend-course")]
        public async Task<IActionResult> RecommendCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.RecommendCourseAsync(userId!);
            return Ok(result);
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
    }
}
