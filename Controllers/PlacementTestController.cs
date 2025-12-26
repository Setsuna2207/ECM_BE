using ECM_BE.Models.DTOs.PlacementTest;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacementTestController : ControllerBase
    {
        private readonly IPlacementTestService _placementTestService;

        public PlacementTestController(IPlacementTestService placementTestService)
        {
            _placementTestService = placementTestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlacementTests()
        {
            var tests = await _placementTestService.GetAllPlacementTestsAsync();
            return Ok(tests);
        }

        [HttpGet("{testId}")]
        public async Task<IActionResult> GetPlacementTestById(int testId)
        {
            try
            {
                var test = await _placementTestService.GetPlacementTestByIdAsync(testId);
                return Ok(test);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreatePlacementTest([FromBody] CreatePlacementTestRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _placementTestService.CreatePlacementTestAsync(requestDto);
                return CreatedAtAction(nameof(GetPlacementTestById), new { testId = result.TestID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{testId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdatePlacementTest(int testId, [FromBody] UpdatePlacementTestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _placementTestService.UpdatePlacementTestAsync(testId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{testId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeletePlacementTest(int testId)
        {
            try
            {
                await _placementTestService.DeletePlacementTestAsync(testId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("upload")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UploadTestFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                // Read file content
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = await reader.ReadToEndAsync();

                    // Parse JSON or JS file
                    List<TestQuestionDTO> questions = null;

                    if (file.FileName.EndsWith(".json"))
                    {
                        // Parse JSON
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(fileContent);
                        questions = JsonConvert.DeserializeObject<List<TestQuestionDTO>>(
                            JsonConvert.SerializeObject(jsonData["questions"])
                        );
                    }
                    else if (file.FileName.EndsWith(".js"))
                    {
                        // Extract object from "export default { ... }"
                        var match = System.Text.RegularExpressions.Regex.Match(
                            fileContent,
                            @"export\s+default\s+({[\s\S]*})"
                        );

                        if (match.Success)
                        {
                            var jsonStr = match.Groups[1].Value;
                            var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                            questions = JsonConvert.DeserializeObject<List<TestQuestionDTO>>(
                                JsonConvert.SerializeObject(jsonData["questions"])
                            );
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

                    // Save file to disk (optional - for reference)
                    var fileName = $"{DateTime.Now.Ticks}-{file.FileName}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(uploadPath));
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok(new
                    {
                        fileUrl = $"/uploads/{fileName}",
                        questions = questions
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
        public async Task<IActionResult> UploadMediaFile(IFormFile file)
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

                return Ok(new
                {
                    mediaUrl = $"/uploads/{fileName}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading media: {ex.Message}");
            }
        }

    }
}