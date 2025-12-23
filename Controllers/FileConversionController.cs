using Asp.Versioning;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    // [Authorize(Policy = "AdminPolicy")] // Temporarily commented
    public class FileConversionController : ControllerBase
    {
        private readonly IFileConversionService _conversionService;

        public FileConversionController(IFileConversionService conversionService)
        {
            _conversionService = conversionService;
        }
        [HttpPost("convert-docx")]
        public async Task<IActionResult> ConvertDocxToJson(
            IFormFile file,             
            [FromForm] string fileType)  
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                if (!file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { success = false, message = "Only .docx files are supported" });
                }

                var result = await _conversionService.ConvertDocxToJsonAsync(file, fileType);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("convert-pdf")]
        public async Task<IActionResult> ConvertPdfToJson(
            IFormFile file,              // ← Removed [FromForm]
            [FromForm] string fileType)  // ← Keep [FromForm] for string
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { success = false, message = "Only .pdf files are supported" });
                }

                var result = await _conversionService.ConvertPdfToJsonAsync(file, fileType);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}