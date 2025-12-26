namespace ECM_BE.Models.DTOs.TestResult
{
    public class UpdateTestResultDTO
    {
        public string UserAnswers { get; set; } = null!; // Store JSON: {"1": 0, "2": 1, "3": 2, ...}
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public int SkippedAnswers { get; set; }
        public float OverallScore { get; set; } // Percentage or total points
        public string? SectionScores { get; set; } // Store JSON: {"listening": 80, "reading": 75, "grammar": 90, ...}
        public string? LevelDetected { get; set; } // e.g., "Beginner", "Intermediate", "Advanced"
        public int? TimeSpent { get; set; }
    }
}
