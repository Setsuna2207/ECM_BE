namespace ECM_BE.Models.DTOs.AIFeedback
{
    public class UpdateAIFeedbackDTO
    {
        public string? WeakSkill { get; set; }
        public string? RcmCourses { get; set; }
        public string? FeedbackSummary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
