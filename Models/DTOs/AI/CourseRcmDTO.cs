namespace ECM_BE.Models.DTOs.AI
{
    public class CourseRcmDTO
    {
        public List<CourseRcmDTO> Recommendations { get; set; } = new();
    }

    public class CourseRcmItemDTO
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
