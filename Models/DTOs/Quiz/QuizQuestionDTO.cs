namespace ECM_BE.Models.DTOs.Quiz
{
    public class QuizQuestionDTO
    {
        public int QuestionId { get; set; }
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswer { get; set; }
    }
}