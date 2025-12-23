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

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseByID(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound("Không tìm thấy khóa học");
            return Ok(course);
        }

        [HttpGet("Category/{categoryId}")]
        public async Task<IActionResult> GetCoursesByCategory(int categoryId)
        {
            var course = await _courseService.GetCoursesByCategoryAsync(categoryId);
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

            try
            {
                var result = await _courseService.CreateCourseAsync(requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{courseId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] UpdateCourse requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _courseService.UpdateCourseAsync(courseId, requestDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{courseId}")]
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
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}