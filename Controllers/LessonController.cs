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

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
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
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Course/{courseId}")]
        public async Task<IActionResult> GetLessonByCourseId(int courseId)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByCourseIdAsync(courseId);
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _lessonService.CreateLessonAsync(requestDto);
                return CreatedAtAction(nameof(GetLessonById), new { lessonId = result.LessonID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{lessonId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateLesson(int lessonId, [FromBody] UpdateLessonDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _lessonService.UpdateLessonAsync(lessonId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{lessonId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            try
            {
                await _lessonService.DeleteLessonAsync(lessonId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}