namespace ECM_BE.Models.DTOs.AIRecommendation
{
    public class CourseRecommendationResponseDTO
    {
        public int LearningPathID { get; set; }
        public List<RecommendedCourseDTO> RecommendedCourses { get; set; } = new List<RecommendedCourseDTO>();
        public string WeakSkills { get; set; } = null!;
        public string FeedbackSummary { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public class RecommendedCourseDTO
    {
        public int CourseID { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public int Priority { get; set; } // 1 = highest priority
    }
}
