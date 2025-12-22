namespace ECM_BE.Models.DTOs.QuizResult
{
    public class UpdateQuizResultDTO
    {
        public string? UserAnswers { get; set; }
        public float? Score { get; set; }
        public int? TotalQuestions { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
