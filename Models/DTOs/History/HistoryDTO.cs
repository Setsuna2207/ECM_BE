namespace ECM_BE.Models.DTOs.History
{
    public class HistoryDTO
    {
        public int HistoryID { get; set; }
        public string? UserID { get; set; }
        public int? CourseID { get; set; }
        public float Progress { get; set; }
        public DateTime? LastAccessed { get; set; }
        public string? CourseTitle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
    }
}
