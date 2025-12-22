using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.TestResult;

namespace ECM_BE.Models.Mapper
{
    public static class TestResultMapper
    {
        public static TestResult ToTestResultFromCreate(this CreateTestResultRequestDTO requestDto)
        {
            return new TestResult
            {
                TestID = requestDto.TestID,
                userID = requestDto.UserID,
                UserAnswers = requestDto.UserAnswers,
                CorrectAnswers = requestDto.CorrectAnswers,
                IncorrectAnswers = requestDto.IncorrectAnswers,
                SkippedAnswers = requestDto.SkippedAnswers,
                OverallScore = requestDto.OverallScore,
                SectionScores = requestDto.SectionScores,
                LevelDetected = requestDto.LevelDetected,
                TimeSpent = requestDto.TimeSpent,
            };
        }
        public static TestResultDTO ToTestResultDto(this TestResult requestDto)
        {
            return new TestResultDTO
            {
                ResultID = requestDto.ResultID,
                TestID = requestDto.TestID,
                UserID = requestDto.userID,
                UserAnswers = requestDto.UserAnswers,
                CorrectAnswers = requestDto.CorrectAnswers,
                IncorrectAnswers = requestDto.IncorrectAnswers,
                SkippedAnswers = requestDto.SkippedAnswers,
                OverallScore = requestDto.OverallScore,
                SectionScores = requestDto.SectionScores,
                LevelDetected = requestDto.LevelDetected,
                TimeSpent = requestDto.TimeSpent,
                CreatedAt = requestDto.CreatedAt,
            };
        }
    }
}
