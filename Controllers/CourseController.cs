using ECM_BE.Exceptions.Custom;
using ECM_BE.Models.DTOs.Course;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCourseAsync();
            return Ok(courses);
        }
        [HttpGet("{CourseID}")]
        public async Task<IActionResult> GetCourseByID(int CourseID)
        {
            var course = await _courseService.GetCourseByIdAsync(CourseID);
            if (course == null) return NotFound("Không tìm thấy khóa học");
            return Ok(course);
        }
        [HttpGet("Category/{CategoryID}")]
        public async Task<IActionResult> GetCoursesByCategory(int CategoryID)
        {
            var course = await _courseService.GetCoursesByCategoryAsync(CategoryID);
            if (course == null) return NotFound("Không tìm thấy khóa học");
            return Ok(course);
        }
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _courseService.CreateCourseAsync(requestDto);
            return Ok(result);
        }
        [HttpPut("{CourseID}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourse requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _courseService.UpdateCourseAsync(id, requestDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Lỗi khi cập nhật khoá học.");
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            try
            {
                await _courseService.DeleteCourseAsync(courseId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Lỗi khi xóa khoá học.");
            }
        }
    }
}
