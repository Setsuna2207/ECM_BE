using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.QuizResult;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizResultController : ControllerBase
    {
        private readonly IQuizResult _quizResultService;
        private readonly UserManager<User> _userManager;

        public QuizResultController(IQuizResult quizResultService, UserManager<User> userManager)
        {
            _quizResultService = quizResultService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllQuizResults()
        {
            var results = await _quizResultService.GetAllQuizResultsAsync();
            return Ok(results);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetQuizResultsByUserId(string userId)
        {
            try
            {
                var username = User.GetUsername();
                var user = await _userManager.FindByNameAsync(username);
                
                if (user == null || user.Id != userId)
                    return Unauthorized("Bạn chỉ có thể xem kết quả của chính mình");

                var results = await _quizResultService.GetQuizResultsByUserIdAsync(userId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{resultId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetQuizResultById(int resultId)
        {
            try
            {
                var result = await _quizResultService.GetQuizResultByIdAsync(resultId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> CreateQuizResult([FromBody] CreateQuizResultRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                requestDto.UserID = user.Id;
                var result = await _quizResultService.CreateQuizResultAsync(requestDto);
                return CreatedAtAction(nameof(GetQuizResultById), new { resultId = result.ResultID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{resultId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateQuizResult(int resultId, [FromBody] UpdateQuizResultDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _quizResultService.UpdateQuizResultAsync(resultId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{resultId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteQuizResult(int resultId)
        {
            try
            {
                await _quizResultService.DeleteQuizResultAsync(resultId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}