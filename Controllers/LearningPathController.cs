using ECM_BE.Extensions;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningPathController : ControllerBase
    {
        private readonly ILearningPathService _learningPathService;
        private readonly IAIFeedbackService _aiFeedbackService;
        private readonly UserManager<User> _userManager;

        public LearningPathController(
            ILearningPathService learningPathService,
            IAIFeedbackService aiFeedbackService,
            UserManager<User> userManager)
        {
            _learningPathService = learningPathService;
            _aiFeedbackService = aiFeedbackService;
            _userManager = userManager;
        }

        [HttpGet("active")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetActiveLearningPath()
        {
            try
            {
                var username = User.GetUsername();
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    return NotFound("Không tìm thấy người dùng");

                var learningPath = await _learningPathService.GetActiveLearningPathByUserIdAsync(user.Id);
                
                if (learningPath == null)
                    return Ok(new { message = "Chưa có lộ trình học tập nào", learningPath = (object?)null });

                // Get AI feedback if initial result exists
                object? aiFeedback = null;
                if (learningPath.InitialResultID.HasValue)
                {
                    try
                    {
                        var feedbacks = await _aiFeedbackService.GetAllAIFeedbacksAsync();
                        aiFeedback = feedbacks.FirstOrDefault(f => f.ResultID == learningPath.InitialResultID.Value);
                    }
                    catch
                    {
                        // AI feedback is optional
                    }
                }

                return Ok(new
                {
                    learningPath,
                    aiFeedback
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{learningPathId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetLearningPathById(int learningPathId)
        {
            try
            {
                var learningPath = await _learningPathService.GetLearningPathByIdAsync(learningPathId);
                return Ok(learningPath);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
