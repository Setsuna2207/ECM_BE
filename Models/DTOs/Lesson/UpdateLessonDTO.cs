namespace ECM_BE.Models.DTOs.Lesson
{
    public class UpdateLessonDTO
    {
        public string? Title { get; set; }
        public string VideoUrl { get; set; } = null!;
        public List<string> DocumentUrl { get; set; } = new List<string>();
        public int? OrderIndex { get; set; }
    }
}
