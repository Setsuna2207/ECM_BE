using ECM_BE.Models.DTOs.PlacementTest;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}