using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadController> _logger;

        // File type configurations
        private static readonly Dictionary<string, string[]> FileTypeConfig = new()
        {
            { "video", new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", ".mkv", ".m4v" } },
            { "document", new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".txt", ".xls", ".xlsx" } }
        };

        // Size limits in bytes
        private const long MaxVideoSize = 5L * 1024 * 1024 * 1024; // 5 GB for videos
        private const long MaxDocumentSize = 100L * 1024 * 1024; // 100 MB for documents

        public FileUploadController(IWebHostEnvironment environment, ILogger<FileUploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Authorize(Policy = "AdminPolicy")]
        [RequestSizeLimit(5_368_709_120)] // 5 GB
        [RequestFormLimits(MultipartBodyLengthLimit = 5_368_709_120)]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string type = "document")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            // Normalize type
            type = type.ToLowerInvariant();
            if (!FileTypeConfig.ContainsKey(type))
                return BadRequest(new { message = "Invalid file type. Use 'video' or 'document'" });

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = FileTypeConfig[type];

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    message = $"Invalid file extension. Allowed extensions for {type}: {string.Join(", ", allowedExtensions)}"
                });
            }

            // Validate file size
            var maxSize = type == "video" ? MaxVideoSize : MaxDocumentSize;
            if (file.Length > maxSize)
            {
                var maxSizeMB = maxSize / (1024.0 * 1024.0);
                return BadRequest(new
                {
                    message = $"File too large. Maximum size for {type}: {maxSizeMB:F0} MB"
                });
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", $"{type}s");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return URL
                var fileUrl = $"/uploads/{type}s/{uniqueFileName}";

                _logger.LogInformation("File uploaded successfully: {FileName} ({Size} bytes)",
                    file.FileName, file.Length);

                return Ok(new
                {
                    url = fileUrl,
                    fileName = file.FileName,
                    size = file.Length,
                    type = type
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading {Type} file: {FileName}", type, file.FileName);
                return StatusCode(500, new { message = "Error uploading file", error = ex.Message });
            }
        }

        [HttpPost("upload/multiple")]
        [Authorize(Policy = "AdminPolicy")]
        [RequestSizeLimit(5_368_709_120)] // 5 GB total
        public async Task<IActionResult> UploadMultipleFiles(List<IFormFile> files, [FromQuery] string type = "document")
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files uploaded" });

            // Normalize type
            type = type.ToLowerInvariant();
            if (!FileTypeConfig.ContainsKey(type))
                return BadRequest(new { message = "Invalid file type. Use 'video' or 'document'" });

            var allowedExtensions = FileTypeConfig[type];
            var maxSize = type == "video" ? MaxVideoSize : MaxDocumentSize;
            var uploadedFiles = new List<object>();
            var errors = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    errors.Add($"File '{file.FileName}' is empty");
                    continue;
                }

                // Validate extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    errors.Add($"File '{file.FileName}' has invalid extension. Allowed: {string.Join(", ", allowedExtensions)}");
                    continue;
                }

                // Validate size
                if (file.Length > maxSize)
                {
                    var maxSizeMB = maxSize / (1024.0 * 1024.0);
                    errors.Add($"File '{file.FileName}' is too large. Max: {maxSizeMB:F0} MB");
                    continue;
                }

                try
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", $"{type}s");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var fileUrl = $"/uploads/{type}s/{uniqueFileName}";

                    uploadedFiles.Add(new
                    {
                        url = fileUrl,
                        fileName = file.FileName,
                        size = file.Length,
                        type = type
                    });

                    _logger.LogInformation("File uploaded successfully: {FileName} ({Size} bytes)",
                        file.FileName, file.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
                    errors.Add($"Error uploading '{file.FileName}': {ex.Message}");
                }
            }

            return Ok(new
            {
                files = uploadedFiles,
                errors = errors.Count > 0 ? errors : null,
                successCount = uploadedFiles.Count,
                errorCount = errors.Count,
                type = type
            });
        }

        [HttpDelete]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult DeleteFile([FromQuery] string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return BadRequest(new { message = "File URL is required" });

            try
            {
                // Remove leading slash if present
                var relativePath = fileUrl.TrimStart('/');
                var filePath = Path.Combine(_environment.WebRootPath, relativePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return Ok(new { message = "File deleted successfully" });
                }
                else
                {
                    _logger.LogWarning("File not found: {FilePath}", filePath);
                    return NotFound(new { message = "File not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
                return StatusCode(500, new { message = "Error deleting file", error = ex.Message });
            }
        }

        [HttpGet("info")]
        public IActionResult GetFileInfo()
        {
            return Ok(new
            {
                limits = new
                {
                    video = new
                    {
                        maxSize = $"{MaxVideoSize / (1024.0 * 1024.0 * 1024.0):F1} GB",
                        maxSizeBytes = MaxVideoSize,
                        allowedExtensions = FileTypeConfig["video"]
                    },
                    document = new
                    {
                        maxSize = $"{MaxDocumentSize / (1024.0 * 1024.0):F0} MB",
                        maxSizeBytes = MaxDocumentSize,
                        allowedExtensions = FileTypeConfig["document"]
                    }
                }
            });
        }
    }
}