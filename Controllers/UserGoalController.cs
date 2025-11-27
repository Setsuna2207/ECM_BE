using ECM_BE.Exceptions.Custom;
using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.UserGoal;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGoalController : ControllerBase
    {
        private readonly IUserGoalService _userGoalService;
        private readonly ILogger<UserGoalController> _logger;
        private readonly UserManager<User> _userManager;

        public UserGoalController(IUserGoalService userGoalService, ILogger<UserGoalController> logger, UserManager<User> userManager)
        {
            _userGoalService = userGoalService;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserGoals()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            try
            {
                var result = await _userGoalService.GetUserGoalsAsync(user.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách mục tiêu học.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserGoal([FromBody] CreateUserGoalRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            try
            {
                var result = await _userGoalService.CreateUserGoalAsync(user.Id, requestDto);
                return CreatedAtAction(nameof(GetUserGoals), new { id = result.UserGoalID }, result);
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Mục tiêu học đã tồn tại.");
                return Conflict(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Dữ liệu mục tiêu học không hợp lệ.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi tạo mục tiêu học.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }


        [HttpPut("{userGoalID}")]
        public async Task<IActionResult> UpdateUserGoal(
            [FromRoute] int userGoalID,
            [FromBody] UpdateUserGoalDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            try
            {
                var result = await _userGoalService.UpdateUserGoalAsync(user.Id, userGoalID, requestDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy mục tiêu học.");
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Danh mục mục tiêu học không hợp lệ.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi cập nhật mục tiêu học.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        [HttpDelete("{userGoalID}")]
        public async Task<IActionResult> DeleteUserGoal([FromRoute] int userGoalID)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            try
            {
                await _userGoalService.DeleteUserGoalAsync(user.Id, userGoalID);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy mục tiêu học để xóa.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi xóa mục tiêu học.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }
    }
}
