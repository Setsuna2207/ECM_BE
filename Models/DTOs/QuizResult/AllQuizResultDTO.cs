namespace ECM_BE.Models.DTOs.QuizResult
{
    public class AllQuizResultDTO
    {
        public int ResultID { get; set; }
        public int QuizID { get; set; }
        public string userID { get; set; } = null!;
        public string? UserAnswers { get; set; }
        public float? Score { get; set; }
        public int? TotalQuestions { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
