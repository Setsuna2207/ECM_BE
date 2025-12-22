using ECM_BE.Data;
using ECM_BE.Models.DTOs.TestResult;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly AppDbContext _context;

        public TestResultService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AllTestResultDTO>> GetAllTestResultsAsync()
        {
            return await _context.TestResults
                .AsNoTracking()
                .Select(tr => new AllTestResultDTO
                {
                    ResultID = tr.ResultID,
                    TestID = tr.TestID,
                    UserID = tr.userID,
                    UserAnswers = tr.UserAnswers,
                    CorrectAnswers = tr.CorrectAnswers,
                    IncorrectAnswers = tr.IncorrectAnswers,
                    SkippedAnswers = tr.SkippedAnswers,
                    OverallScore = tr.OverallScore,
                    SectionScores = tr.SectionScores,
                    LevelDetected = tr.LevelDetected,
                    TimeSpent = tr.TimeSpent,
                    CreatedAt = tr.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TestResultDTO> GetTestResultByIdAsync(int testResultId)
        {
            var tr = await _context.TestResults
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ResultID == testResultId);

            if (tr == null)
                throw new Exception("Không tìm thấy kết quả test");

            return tr.ToTestResultDto();
        }

        public async Task<TestResultDTO> CreateTestResultAsync(CreateTestResultRequestDTO requestDto)
        {
            var tr = requestDto.ToTestResultFromCreate();
            _context.TestResults.Add(tr);
            await _context.SaveChangesAsync();

            return tr.ToTestResultDto();
        }

        public async Task<TestResultDTO> UpdateTestResultAsync(int ResultId, UpdateTestResultDTO requestDto)
        {
            var tr = await _context.TestResults
                .FirstOrDefaultAsync(x => x.ResultID == ResultId);

            if (tr == null)
                throw new Exception("Không tìm thấy kết quả test");

            tr.UserAnswers = requestDto.UserAnswers;
            tr.CorrectAnswers = requestDto.CorrectAnswers;
            tr.IncorrectAnswers = requestDto.IncorrectAnswers;
            tr.SkippedAnswers = requestDto.SkippedAnswers;
            tr.OverallScore = requestDto.OverallScore;
            tr.SectionScores = requestDto.SectionScores;
            tr.LevelDetected = requestDto.LevelDetected;
            tr.TimeSpent = requestDto.TimeSpent;

            await _context.SaveChangesAsync();

            return tr.ToTestResultDto();
        }

        public async Task DeleteTestResultAsync(int testResultId)
        {
            var tr = await _context.TestResults
                .FirstOrDefaultAsync(x => x.ResultID == testResultId);

            if (tr == null)
                throw new Exception("Test result not found");

            _context.TestResults.Remove(tr);
            await _context.SaveChangesAsync();
        }
    }
}
