using ECM_BE.Models.DTOs.QuizResult;

namespace ECM_BE.Services.Interfaces
{
    public interface IQuizResult
    {
        Task<List<AllQuizResultDTO>> GetAllQuizResultsAsync();
        Task<QuizResultDTO> GetQuizResultByIdAsync(int ResultId);
        Task<QuizResultDTO> CreateQuizResultAsync(CreateQuizResultRequestDTO requestDto);
        Task<QuizResultDTO> UpdateQuizResultAsync(int ResultId, UpdateQuizResultDTO requestDto);
        Task DeleteQuizResultAsync(int ResultId);
    }
}
