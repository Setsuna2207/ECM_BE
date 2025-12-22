using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.Quiz;

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
            };
        }
        public static QuizDTO ToQuizDto(this Quiz requestDto)
        {
            return new QuizDTO
            {
                QuizID = requestDto.QuizID,
                LessonID = requestDto.LessonID,
                Description = requestDto.Description,
                QuestionFileUrl = requestDto.QuestionFileUrl,
                MediaUrl = requestDto.MediaUrl,
            };
        }
    }
}
