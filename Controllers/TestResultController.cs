using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.TestResult;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultService _testResultService;
        private readonly UserManager<User> _userManager;

        public TestResultController(ITestResultService testResultService, UserManager<User> userManager)
        {
            _testResultService = testResultService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllTestResults()
        {
            var results = await _testResultService.GetAllTestResultsAsync();
            return Ok(results);
        }

        [HttpGet("{resultId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetTestResultById(int resultId)
        {
            try
            {
                var result = await _testResultService.GetTestResultByIdAsync(resultId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> CreateTestResult([FromBody] CreateTestResultRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                requestDto.UserID = user.Id;
                var result = await _testResultService.CreateTestResultAsync(requestDto);
                return CreatedAtAction(nameof(GetTestResultById), new { resultId = result.ResultID }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{resultId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateTestResult(int resultId, [FromBody] UpdateTestResultDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _testResultService.UpdateTestResultAsync(resultId, requestDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{resultId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteTestResult(int resultId)
        {
            try
            {
                await _testResultService.DeleteTestResultAsync(resultId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}