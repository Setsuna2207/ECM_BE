using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowingController : ControllerBase
    {
        private readonly IFollowingService _followingService;
        private readonly UserManager<User> _userManager;

        public FollowingController(IFollowingService followingService, UserManager<User> userManager)
        {
            _followingService = followingService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetAllFollowing()
        {
            var userName = User.Identity?.Name;
            var user = await _userManager.FindByNameAsync(userName); // Lấy user từ UserManager
            var followings = await _followingService.GetAllFollowingsCourse(user.Id);
            return Ok(followings);
        }
        [HttpPost("toggle/{courseId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> ToggleFollowing(int courseId)
        {

            var userName = User.Identity?.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var isFollowing = await _followingService.ToggleFollowingAsync(user.Id, courseId);
            return Ok(new { status = isFollowing ? "Added" : "Removed" });
        }
        [HttpDelete("{courseId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> RemoveFollowing(int courseId)
        {
            var userName = User.Identity?.Name;
            var user = await _userManager.FindByNameAsync(userName);
            await _followingService.RemoveFollowingAsync(user.Id, courseId);
            return NoContent();
        }
    }
}
