namespace ECM_BE.Models.DTOs.Quiz
{
    public class QuizDTO
    {
        public int QuizID { get; set; }
        public int LessonID { get; set; }
        public string? QuestionFileUrl { get; set; }
        public string? MediaUrl { get; set; }
        public string? Description { get; set; }
        public List<QuizQuestionDTO>? Questions { get; set; }
    }
}
