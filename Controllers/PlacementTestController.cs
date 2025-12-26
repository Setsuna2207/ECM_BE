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
            try
            {
                var tests = await _placementTestService.GetAllPlacementTestsAsync();

                // Map to include all necessary fields
                var result = tests.Select(t => new
                {
                    t.TestID,
                    t.Title,
                    t.Description,
                    t.Duration,
                    t.TotalQuestions,
                    t.QuestionFileURL,
                    t.MediaURL,
                    t.Sections
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching tests: {ex.Message}");
            }
        }

        [HttpGet("{testId}")]
        public async Task<IActionResult> GetPlacementTestById(int testId)
        {
            try
            {
                var test = await _placementTestService.GetPlacementTestByIdAsync(testId);
                if (test == null)
                    return NotFound("Test not found");

                return Ok(new
                {
                    test.TestID,
                    test.Title,
                    test.Description,
                    test.Duration,
                    test.TotalQuestions,
                    test.QuestionFileURL,
                    test.MediaURL,
                    test.Sections
                });
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
        public async Task<IActionResult> UploadTestFile(IFormFile file, [FromQuery] string title, [FromQuery] string? description = "", [FromQuery] int? testId = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded" });

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = await reader.ReadToEndAsync();
                    List<TestSectionDTO>? sections = null;

                    // Parse file based on extension
                    if (file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        // Parse JSON directly
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(fileContent);

                        // Check if it has sections property
                        if (jsonData?.sections != null)
                        {
                            sections = JsonConvert.DeserializeObject<List<TestSectionDTO>>(
                                JsonConvert.SerializeObject(jsonData.sections));
                        }
                    }
                    else if (file.FileName.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract the object from "export default { ... }"
                        var match = System.Text.RegularExpressions.Regex.Match(
                            fileContent,
                            @"export\s+default\s+(\{[\s\S]*\});?\s*$",
                            System.Text.RegularExpressions.RegexOptions.Multiline
                        );

                        if (!match.Success)
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = "Invalid .js file format. Must contain 'export default { ... }'"
                            });
                        }

                        var jsonContent = match.Groups[1].Value;

                        // Remove trailing semicolon and comma if present
                        jsonContent = jsonContent.TrimEnd(';', ',').Trim();

                        try
                        {
                            // Parse the JavaScript object as JSON
                            var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonContent);

                            if (jsonData?.sections != null)
                            {
                                sections = JsonConvert.DeserializeObject<List<TestSectionDTO>>(
                                    JsonConvert.SerializeObject(jsonData.sections));
                            }
                        }
                        catch (JsonException ex)
                        {
                            return BadRequest(new
                            {
                                success = false,
                                message = $"Error parsing JavaScript object: {ex.Message}. The JS object must be valid JSON format."
                            });
                        }
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "File must be .js or .json format"
                        });
                    }

                    // Validate sections
                    if (sections == null || sections.Count == 0)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "No sections found in file. Expected format: { sections: [...] }"
                        });
                    }

                    // Validate that sections have questions
                    var totalQuestions = sections.Sum(s => s.Questions?.Count ?? 0);
                    if (totalQuestions == 0)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "No questions found in sections"
                        });
                    }

                    // Calculate total duration (default 15 minutes per section if not specified)
                    int totalDuration = sections.Sum(s => s.Duration > 0 ? s.Duration : 15);

                    PlacementTestDTO result;

                    if (testId.HasValue && testId.Value > 0)
                    {
                        // Update existing test
                        var updateDto = new UpdatePlacementTestDTO
                        {
                            Title = title,
                            Description = description ?? "",
                            Duration = totalDuration,
                            TotalQuestions = totalQuestions,
                            QuestionFileURL = file.FileName,
                            MediaURL = "",
                            Sections = sections
                        };
                        result = await _placementTestService.UpdatePlacementTestAsync(testId.Value, updateDto);
                    }
                    else
                    {
                        // Create new test
                        var createDto = new CreatePlacementTestRequestDTO
                        {
                            Title = title,
                            Description = description ?? "",
                            Duration = totalDuration,
                            TotalQuestions = totalQuestions,
                            QuestionFileURL = file.FileName,
                            MediaURL = "",
                            Sections = sections
                        };
                        result = await _placementTestService.CreatePlacementTestAsync(createDto);
                    }

                    return Ok(new
                    {
                        success = true,
                        message = $"Test uploaded successfully ({sections.Count} sections, {totalQuestions} questions)",
                        data = new
                        {
                            testId = result.TestID,
                            title = result.Title,
                            description = result.Description,
                            duration = totalDuration,
                            totalQuestions = totalQuestions,
                            sectionsCount = sections.Count,
                            sections = result.Sections
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error processing file: {ex.Message}",
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("{testId}/upload-media")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UploadTestMediaFile(int testId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                // Create uploads directory
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "test-media");
                Directory.CreateDirectory(uploadFolder);

                // Generate unique filename
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{DateTime.Now.Ticks}-test-{testId}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var mediaUrl = $"/uploads/test-media/{fileName}";

                // Update test with media URL
                var existingTest = await _placementTestService.GetPlacementTestByIdAsync(testId);
                if (existingTest == null)
                    return NotFound("Test not found");

                var updateDto = new UpdatePlacementTestDTO
                {
                    Title = existingTest.Title,
                    Description = existingTest.Description,
                    Duration = existingTest.Duration,
                    TotalQuestions = existingTest.TotalQuestions,
                    QuestionFileURL = existingTest.QuestionFileURL,
                    MediaURL = mediaUrl,
                    Sections = existingTest.Sections
                };

                await _placementTestService.UpdatePlacementTestAsync(testId, updateDto);

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