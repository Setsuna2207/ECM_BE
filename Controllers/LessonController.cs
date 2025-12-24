using ECM_BE.Models.DTOs.Lesson;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonController> _logger;

        public LessonController(ILessonService lessonService, ILogger<LessonController> logger)
        {
            _lessonService = lessonService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLessons()
        {
            var lessons = await _lessonService.GetAllLessonsAsync();
            return Ok(lessons);
        }

        [HttpGet("{lessonId}")]
        public async Task<IActionResult> GetLessonById(int lessonId)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lesson by ID: {LessonId}", lessonId);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("Course/{courseId}")]
        public async Task<IActionResult> GetLessonByCourseId(int courseId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonByCourseIdAsync(courseId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lessons for course: {CourseId}", courseId);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Validation failed for CreateLesson: {Errors}", string.Join(", ", errors));
                return BadRequest(new { message = "Validation failed", errors = errors });
            }

            try
            {
                _logger.LogInformation("Creating lesson: {@RequestDto}", requestDto);

                var result = await _lessonService.CreateLessonAsync(requestDto);

                _logger.LogInformation("Lesson created successfully: {LessonId}", result.LessonID);

                return CreatedAtAction(nameof(GetLessonById), new { lessonId = result.LessonID }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson: {@RequestDto}", requestDto);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{lessonId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] UpdateLessonDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning("Validation failed for UpdateLesson: {Errors}", string.Join(", ", errors));
                return BadRequest(new { message = "Validation failed", errors = errors });
            }

            try
            {
                _logger.LogInformation("Updating lesson {LessonId}: {@RequestDto}", lessonId, requestDto);

                var result = await _lessonService.UpdateLessonAsync(lessonId, requestDto);

                _logger.LogInformation("Lesson updated successfully: {LessonId}", lessonId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson {LessonId}: {@RequestDto}", lessonId, requestDto);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{lessonId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            try
            {
                _logger.LogInformation("Deleting lesson: {LessonId}", lessonId);

                await _lessonService.DeleteLessonAsync(lessonId);

                _logger.LogInformation("Lesson deleted successfully: {LessonId}", lessonId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson: {LessonId}", lessonId);
                return NotFound(new { message = ex.Message });
            }
        }
    }
}