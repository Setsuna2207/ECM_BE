using ECM_BE.Models.DTOs.AIFeedback;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIFeedbackController : ControllerBase
    {
        private readonly IAIFeedbackService _aiFeedbackService;

        public AIFeedbackController(IAIFeedbackService aiFeedbackService)
        {
            _aiFeedbackService = aiFeedbackService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllAIFeedbacks()
        {
            var feedbacks = await _aiFeedbackService.GetAllAIFeedbacksAsync();
            return Ok(feedbacks);
        }

        [HttpGet("{feedbackId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetAIFeedbackById(int feedbackId)
        {
            try
            {
                var feedback = await _aiFeedbackService.GetAIFeedbackByIdAsync(feedbackId);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> CreateAIFeedback([FromBody] CreateAIFeedbackRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _aiFeedbackService.CreateAIFeedbackAsync(requestDto);
                return CreatedAtAction(nameof(GetAIFeedbackById), new { feedbackId = result.FeedbackID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{feedbackId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateAIFeedback(int feedbackId, [FromBody] UpdateAIFeedbackDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _aiFeedbackService.UpdateAIFeedbackAsync(feedbackId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{feedbackId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteAIFeedback(int feedbackId)
        {
            try
            {
                await _aiFeedbackService.DeleteAIFeedbackAsync(feedbackId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}