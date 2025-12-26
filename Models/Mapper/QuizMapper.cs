using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.Quiz;
using Newtonsoft.Json;

namespace ECM_BE.Models.Mapper
{
    public static class QuizMapper
    {
        public static Quiz ToQuizFromCreate(this CreateQuizRequestDTO requestDto)
        {
            return new Quiz
            {
                LessonID = requestDto.LessonID,
                Description = requestDto.Description,
                QuestionFileUrl = requestDto.QuestionFileUrl,
                MediaUrl = requestDto.MediaUrl,
                Questions = requestDto.Questions != null
                    ? JsonConvert.SerializeObject(requestDto.Questions)
                    : null
            };
        }

        public static QuizDTO ToQuizDto(this Quiz quiz)
        {
            return new QuizDTO
            {
                QuizID = quiz.QuizID,
                LessonID = quiz.LessonID,
                Description = quiz.Description,
                QuestionFileUrl = quiz.QuestionFileUrl,
                MediaUrl = quiz.MediaUrl,
                Questions = !string.IsNullOrEmpty(quiz.Questions)
                    ? JsonConvert.DeserializeObject<List<QuizQuestionDTO>>(quiz.Questions)
                    : null
            };
        }
    }
}