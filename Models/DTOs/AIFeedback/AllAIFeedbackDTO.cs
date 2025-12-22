namespace ECM_BE.Models.DTOs.AIFeedback
{
    public class AllAIFeedbackDTO
    {
        public int FeedbackID { get; set; }
        public int ResultID { get; set; }
        public string? WeakSkill { get; set; }
        public string? RcmCourses { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FeedbackSummary { get; set; }
    }
}
