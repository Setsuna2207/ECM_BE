using ECM_BE.Data;
using ECM_BE.Models.DTOs.Quiz;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class QuizService : IQuizService
    {
        private readonly AppDbContext _context;
        public QuizService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<AllQuizDTO>> GetAllQuizzesAsync()
        {
            return await _context.Quizzes
                .AsNoTracking()
                .Select(q => new AllQuizDTO
                {
                    QuizID = q.QuizID,
                    LessonID = q.LessonID,
                    QuestionFileUrl = q.QuestionFileUrl,
                    MediaUrl = q.MediaUrl,
                    Description = q.Description
                })
                .ToListAsync();
        }
        public async Task<QuizDTO> GetQuizByIdAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.QuizID == quizId);
            if (quiz == null)
                throw new Exception("Không tìm thấy bài quiz");
            return quiz.ToQuizDto();
        }
        public async Task<QuizDTO> CreateQuizAsync(CreateQuizRequestDTO requestDto)
        {
            var quiz = requestDto.ToQuizFromCreate();
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz.ToQuizDto();
        }
        public async Task<QuizDTO> UpdateQuizAsync(int quizId, UpdateQuizDTO requestDto)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizID == quizId);
            if (quiz == null)
                throw new Exception("Không tìm thấy bài quiz");

            quiz.LessonID = requestDto.LessonID;
            quiz.QuestionFileUrl = requestDto.QuestionFileUrl;
            quiz.MediaUrl = requestDto.MediaUrl;
            quiz.Description = requestDto.Description;

            await _context.SaveChangesAsync();
            return quiz.ToQuizDto();
        }
        public async Task DeleteQuizAsync(int quizId)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizID == quizId);
            if (quiz == null)
                throw new Exception("Không tìm thấy bài quiz");
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
    }
}
