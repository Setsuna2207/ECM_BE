using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.Review;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<User> _userManager;

        public ReviewController(IReviewService reviewService, UserManager<User> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpGet] //Lấy tất cả đánh giá
        public async Task<IActionResult> GetAllReview()
        {
            var reviews = await _reviewService.GetAllReviewAsync();
            return Ok(reviews);
        }

        [HttpGet("Course/{CourseID}")] //Lấy đánh giá theo CourseID
        public async Task<IActionResult> GetReviewByCourseID(int CourseID)
        {
            var review = await _reviewService.GetReviewByCourseIDAsync(CourseID);
            if (review == null) return NotFound("Không tìm thấy đánh giá");
            return Ok(review);
        }

        [HttpGet("user/{userID}")] //Lấy đánh giá theo userID
        public async Task<IActionResult> GetReviewByuserID(string userID)
        {
            var review = await _reviewService.GetReviewByuserIDAsync(userID);
            if (review == null) return NotFound("Không tìm thấy đánh giá");
            return Ok(review);
        }

        [HttpPost] //Tạo đánh giá
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDTO requestDto)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _reviewService.CreateReviewAsync(user.Id, requestDto);
            return Ok(result);
        }

        [HttpPut("{CourseID}")] // Cập nhật đánh giá
        public async Task<IActionResult> UpdateReview(int CourseID, [FromBody] UpdateReviewDTO requestDto)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _reviewService.UpdateReviewAsync(CourseID, requestDto, userID);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{CourseID}")] //Xóa đánh giá
        public async Task<IActionResult> DeleteReview(int CourseID)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin"); // Kiểm tra xem người dùng có phải admin không
            await _reviewService.DeleteReviewAsync(CourseID, userID, isAdmin);
            return Ok("Review deleted successfully");
        }
    }
}
