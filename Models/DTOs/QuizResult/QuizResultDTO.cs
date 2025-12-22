namespace ECM_BE.Models.DTOs.QuizResult
{
    public class QuizResultDTO
    {
        public int ResultID { get; set; }
        public int QuizID { get; set; }
        public string UserID { get; set; } = null!;
        public string? UserAnswers { get; set; }
        public float? Score { get; set; }
        public int? TotalQuestions { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
