using ECM_BE.Models.DTOs.FileConversion;

namespace ECM_BE.Services.Interfaces
{
    public interface IFileConversionService
    {
        Task<ConversionResultDTO> ConvertDocxToJsonAsync(IFormFile file, string fileType);
        Task<ConversionResultDTO> ConvertPdfToJsonAsync(IFormFile file, string fileType);
        Task<ConversionResultDTO> ParseQuizContentAsync(string content);
        Task<ConversionResultDTO> ParseTestContentAsync(string content);
    }
}