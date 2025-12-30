namespace ECM_BE.Models.DTOs.LearningPath
{
    public class LearningPathDTO
    {
        public int LearningPathID { get; set; }
        public string UserID { get; set; } = null!;
        public int UserGoalID { get; set; }
        public string? GoalContent { get; set; }
        public int? InitialTestID { get; set; }
        public string? InitialTestTitle { get; set; }
        public int? InitialResultID { get; set; }
        public string? RecommendedCourses { get; set; }
        public string? CompletedCourses { get; set; }
        public int? FinalTestID { get; set; }
        public string? FinalTestTitle { get; set; }
        public int? FinalResultID { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
