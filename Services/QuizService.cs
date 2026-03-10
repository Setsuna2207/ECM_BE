using ECM_BE.Data;
using ECM_BE.Models.DTOs.Quiz;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                    Description = q.Description,
                    Questions = !string.IsNullOrEmpty(q.Questions)
                        ? JsonConvert.DeserializeObject<List<QuizQuestionDTO>>(q.Questions)
                        : null
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
            Console.WriteLine($"[UpdateQuiz] Updating quiz ID: {quizId}");
            Console.WriteLine($"[UpdateQuiz] MediaUrl in request: {requestDto.MediaUrl}");
            
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizID == quizId);
            if (quiz == null)
            {
                Console.WriteLine($"[UpdateQuiz] Quiz not found: {quizId}");
                throw new Exception("Không tìm thấy bài quiz");
            }

            Console.WriteLine($"[UpdateQuiz] Current MediaUrl: {quiz.MediaUrl}");

            quiz.LessonID = requestDto.LessonID;
            quiz.QuestionFileUrl = requestDto.QuestionFileUrl;
            quiz.MediaUrl = requestDto.MediaUrl;
            quiz.Description = requestDto.Description;
            quiz.Questions = requestDto.Questions != null
                ? JsonConvert.SerializeObject(requestDto.Questions)
                : quiz.Questions;

            Console.WriteLine($"[UpdateQuiz] New MediaUrl: {quiz.MediaUrl}");

            // Explicitly mark the entity as modified to ensure EF tracks the changes
            _context.Entry(quiz).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            
            Console.WriteLine($"[UpdateQuiz] Quiz updated successfully. Changes saved: {changes}");
            
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