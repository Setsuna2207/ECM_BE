using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.UserGoal;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGoalController : ControllerBase
    {
        private readonly IUserGoalService _userGoalService;
        private readonly UserManager<User> _userManager;

        public UserGoalController(IUserGoalService userGoalService, UserManager<User> userManager)
        {
            _userGoalService = userGoalService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllUserGoals()
        {
            var goals = await _userGoalService.GetAllUserGoalsAsync();
            return Ok(goals);
        }

        [HttpGet("{goalId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetUserGoalById(int goalId)
        {
            try
            {
                var goal = await _userGoalService.GetUserGoalByIdAsync(goalId);
                return Ok(goal);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> CreateUserGoal([FromBody] CreateUserGoalRequestDTO requestDto)
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
                var result = await _userGoalService.CreateUserGoalAsync(requestDto);
                return CreatedAtAction(nameof(GetUserGoalById), new { goalId = result.UserGoalID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{goalId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateUserGoal(int goalId, [FromBody] UpdateUserGoalDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userGoalService.UpdateUserGoalAsync(goalId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{goalId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> DeleteUserGoal(int goalId)
        {
            try
            {
                await _userGoalService.DeleteUserGoalAsync(goalId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}