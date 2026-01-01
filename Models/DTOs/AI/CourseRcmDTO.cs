namespace ECM_BE.Models.DTOs.AI
{
    public class CourseRcmDTO
    {
        public List<CourseRcmItemDTO> Recommendations { get; set; } = new();
    }

    public class CourseRcmItemDTO
    {
        public int CourseId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
