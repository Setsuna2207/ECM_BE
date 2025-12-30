using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.AIRecommendation;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIRecommendationController : ControllerBase
    {
        private readonly IAIRecommendationService _aiRecommendationService;
        private readonly UserManager<User> _userManager;

        public AIRecommendationController(IAIRecommendationService aiRecommendationService, UserManager<User> userManager)
        {
            _aiRecommendationService = aiRecommendationService;
            _userManager = userManager;
        }

        [HttpPost("analyze-goal")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> AnalyzeGoalAndRecommendTest()
        {
            try
            {
                var username = User.GetUsername();
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    return NotFound("Không tìm thấy người dùng");

                var request = new GoalAnalysisRequestDTO
                {
                    UserID = user.Id
                };

                var result = await _aiRecommendationService.AnalyzeGoalAndRecommendTestAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("recommend-courses")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> RecommendCoursesBasedOnResult([FromBody] CourseRecommendationRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _aiRecommendationService.RecommendCoursesBasedOnResultAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("generate-final-test/{learningPathId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GenerateFinalAssessment(int learningPathId)
        {
            try
            {
                var result = await _aiRecommendationService.GenerateFinalAssessmentAsync(learningPathId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("update-course-completion")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateCourseCompletion([FromBody] UpdateCourseCompletionDTO request)
        {
            try
            {
                var result = await _aiRecommendationService.UpdateCourseCompletionAsync(request.LearningPathID, request.CourseID);
                return Ok(new { success = result, message = "Đã cập nhật tiến độ học tập" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class UpdateCourseCompletionDTO
    {
        public int LearningPathID { get; set; }
        public int CourseID { get; set; }
    }
}
