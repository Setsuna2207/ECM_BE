using ECM_BE.Exceptions.Custom;
using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.History;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;
        private readonly ILogger<HistoryController> _logger;
        private readonly UserManager<User> _userManager;

        public HistoryController(IHistoryService historyService, ILogger<HistoryController> logger, UserManager<User> userManager)
        {
            _historyService = historyService;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetHistories()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                var result = await _historyService.GetHistoriesAsync(user.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định xảy ra trong HistoryController.GetHistories");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        [HttpPost("{courseId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> CreateHistory([FromRoute] int courseId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                var result = await _historyService.CreateHistoryAsync(user.Id, courseId);
                return CreatedAtAction(nameof(GetHistories), new { id = result.HistoryID }, result.ToHistoryDTO());
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Khóa học không tồn tại.");
                return BadRequest("Không tìm thấy khóa học");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong CreateHistory");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        // Cập nhật thời gian
        [HttpPut("{courseId}/access")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateHistoryAccess([FromRoute] int courseId)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                var result = await _historyService.UpdateHistoryAsync(user.Id, courseId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Không tìm thấy bản ghi lịch sử học.");
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Không tìm thấy khóa học.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong UpdateHistoryAccess.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        // Cập nhật tiến độ
        [HttpPut("{courseId}/progress")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateProgress([FromRoute] int courseId)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                var result = await _historyService.UpdateProgressAsync(user.Id, courseId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Không tìm thấy bản ghi lịch sử học.");
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Không tìm thấy khóa học.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong UpdateProgress.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        [HttpDelete("{courseId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> DeleteHistory([FromRoute] int courseId)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                await _historyService.DeleteHistoryAsync(user.Id, courseId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Không tìm thấy bản ghi cần xóa.");
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Không tìm thấy khóa học.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong DeleteHistory.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        [HttpDelete]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> DeleteAllHistories()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            try
            {
                await _historyService.DeleteAllHistoriesAsync(user.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong DeleteAllHistories.");
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }
    }
}
