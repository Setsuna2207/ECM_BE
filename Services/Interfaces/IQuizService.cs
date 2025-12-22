using ECM_BE.Models.DTOs.Quiz;

namespace ECM_BE.Services.Interfaces
{
    public interface IQuizService
    {
        Task<List<AllQuizDTO>> GetAllQuizzesAsync();
        Task<QuizDTO> GetQuizByIdAsync(int quizId);
        Task<QuizDTO> CreateQuizAsync(CreateQuizRequestDTO requestDto);
        Task<QuizDTO> UpdateQuizAsync(int quizId, UpdateQuizDTO requestDto);
        Task DeleteQuizAsync(int quizId);
    }
}
