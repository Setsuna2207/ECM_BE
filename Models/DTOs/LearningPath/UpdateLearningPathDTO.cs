namespace ECM_BE.Models.DTOs.LearningPath
{
    public class UpdateLearningPathDTO
    {
        public int? InitialTestID { get; set; }
        public int? InitialResultID { get; set; }
        public string? RecommendedCourses { get; set; }
        public string? CompletedCourses { get; set; }
        public int? FinalTestID { get; set; }
        public int? FinalResultID { get; set; }
        public string? Status { get; set; }
    }
}
