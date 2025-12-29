namespace ECM_BE.Models.DTOs.QuizResult
{
    public class CreateQuizResultRequestDTO
    {
        public int QuizID { get; set; }
        public string? UserID { get; set; } // Make it nullable since controller sets it
        public string? UserAnswers { get; set; }
        public float? Score { get; set; }
        public int? TotalQuestions { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
