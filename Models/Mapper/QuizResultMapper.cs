using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.QuizResult;

namespace ECM_BE.Models.Mapper
{
    public static class QuizResultMapper
    {
        public static QuizResult ToQuizResultFromCreate(this CreateQuizResultRequestDTO requestDto, string userID)
        {
            return new QuizResult
            {
                QuizID = requestDto.QuizID,
                userID = userID,
                UserAnswers = requestDto.UserAnswers,
                Score = requestDto.Score,
                TotalQuestions = requestDto.TotalQuestions,
                SubmittedAt = requestDto.SubmittedAt,
            };
        }
        public static QuizResultDTO ToQuizResultDto(this QuizResult requestDto)
        {
            return new QuizResultDTO
            {
                ResultID = requestDto.ResultID,
                QuizID = requestDto.QuizID,
                UserID = requestDto.userID,
                UserAnswers = requestDto.UserAnswers,
                Score = requestDto.Score,
                TotalQuestions = requestDto.TotalQuestions,
                SubmittedAt = requestDto.SubmittedAt,
            };
        }
    }
}
