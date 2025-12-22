using ECM_BE.Data;
using ECM_BE.Models.DTOs.QuizResult;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class QuizResultService : IQuizResult
    {
        private readonly AppDbContext _context;

        public QuizResultService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<AllQuizResultDTO>> GetAllQuizResultsAsync()
        {
            var results = await _context.QuizResults
                .Select(x => new AllQuizResultDTO
                {
                    ResultID = x.ResultID,
                    QuizID = x.QuizID,
                    userID = x.userID,
                    Score = x.Score,
                    TotalQuestions = x.TotalQuestions,
                    SubmittedAt = x.SubmittedAt
                })
                .ToListAsync();

            return results;
        }
        public async Task<QuizResultDTO> GetQuizResultByIdAsync(int quizResultId)
        {
            var quizResult = await _context.QuizResults
                .FirstOrDefaultAsync(x => x.ResultID == quizResultId);

            if (quizResult == null)
                throw new Exception("Quiz Result not found");

            return quizResult.ToQuizResultDto();
        }
        public async Task<QuizResultDTO> CreateQuizResultAsync(CreateQuizResultRequestDTO requestDto)
        {
            string userId = requestDto.UserID ?? "UnknownUser";

            var quizResult = requestDto.ToQuizResultFromCreate(userId);

            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();

            return quizResult.ToQuizResultDto();
        }
        public async Task<QuizResultDTO> UpdateQuizResultAsync(int quizResultId, UpdateQuizResultDTO requestDto)
        {
            var quizResult = await _context.QuizResults
                .FirstOrDefaultAsync(x => x.ResultID == quizResultId);

            if (quizResult == null)
                throw new Exception("Quiz Result not found");

            quizResult.UserAnswers = requestDto.UserAnswers;
            quizResult.Score = requestDto.Score;
            quizResult.TotalQuestions = requestDto.TotalQuestions;

            await _context.SaveChangesAsync();

            return quizResult.ToQuizResultDto();
        }
        public async Task DeleteQuizResultAsync(int quizResultId)
        {
            var quizResult = await _context.QuizResults
                .FirstOrDefaultAsync(x => x.ResultID == quizResultId);

            if (quizResult == null)
                throw new Exception("Quiz Result not found");

            _context.QuizResults.Remove(quizResult);
            await _context.SaveChangesAsync();
        }
    }
}
