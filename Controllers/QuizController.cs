using ECM_BE.Models.DTOs.Quiz;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(int quizId)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(quizId);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _quizService.CreateQuizAsync(requestDto);
                return CreatedAtAction(nameof(GetQuizById), new { quizId = result.QuizID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{quizId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] UpdateQuizDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _quizService.UpdateQuizAsync(quizId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{quizId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            try
            {
                await _quizService.DeleteQuizAsync(quizId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("upload")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UploadQuizFile(IFormFile file, [FromQuery] int lessonId, [FromQuery] string description = "", [FromQuery] int? quizId = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = await reader.ReadToEndAsync();
                    List<QuizQuestionDTO> questions = null;

                    // Parse file
                    if (file.FileName.EndsWith(".json"))
                    {
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(fileContent);
                        questions = JsonConvert.DeserializeObject<List<QuizQuestionDTO>>(
                            JsonConvert.SerializeObject(jsonData["questions"]));
                    }
                    else if (file.FileName.EndsWith(".js"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(fileContent,
                            @"export\s+default\s+({[\s\S]*})");
                        if (match.Success)
                        {
                            var jsonStr = match.Groups[1].Value;
                            var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                            questions = JsonConvert.DeserializeObject<List<QuizQuestionDTO>>(
                                JsonConvert.SerializeObject(jsonData["questions"]));
                        }
                        else
                        {
                            return BadRequest("Invalid .js file format");
                        }
                    }
                    else
                    {
                        return BadRequest("File must be .js or .json format");
                    }

                    if (questions == null || questions.Count == 0)
                        return BadRequest("No questions found in file");

                    QuizDTO result;

                    if (quizId.HasValue && quizId.Value > 0)
                    {
                        // Update existing quiz
                        var updateDto = new UpdateQuizDTO
                        {
                            LessonID = lessonId,
                            QuestionFileUrl = "",
                            MediaUrl = "",
                            Description = description,
                            Questions = questions
                        };
                        result = await _quizService.UpdateQuizAsync(quizId.Value, updateDto);
                    }
                    else
                    {
                        // Create new quiz
                        var createDto = new CreateQuizRequestDTO
                        {
                            LessonID = lessonId,
                            QuestionFileUrl = "",
                            MediaUrl = "",
                            Description = description,
                            Questions = questions
                        };
                        result = await _quizService.CreateQuizAsync(createDto);
                    }

                    return Ok(new
                    {
                        quizId = result.QuizID,
                        lessonId = result.LessonID,
                        questions = questions,
                        description = result.Description
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing file: {ex.Message}");
            }
        }

        [HttpPost("upload-media")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UploadMediaFile(IFormFile file, [FromQuery] int quizId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                var fileName = $"{DateTime.Now.Ticks}-{file.FileName}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));

                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var mediaUrl = $"/uploads/{fileName}";

                // Get existing quiz
                var existingQuiz = await _quizService.GetQuizByIdAsync(quizId);
                if (existingQuiz == null)
                    return NotFound("Quiz not found");

                // Create update DTO with only the fields we need
                var updateDto = new UpdateQuizDTO
                {
                    LessonID = existingQuiz.LessonID,
                    QuestionFileUrl = existingQuiz.QuestionFileUrl ?? "",
                    MediaUrl = mediaUrl,
                    Description = existingQuiz.Description ?? "",
                    Questions = null  // Don't touch questions
                };

                await _quizService.UpdateQuizAsync(quizId, updateDto);

                return Ok(new
                {
                    mediaUrl = mediaUrl,
                    fileName = file.FileName
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing media: {ex.Message}");
            }
        }

    }
}