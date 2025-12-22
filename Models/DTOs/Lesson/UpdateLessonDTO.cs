namespace ECM_BE.Models.DTOs.Lesson
{
    public class UpdateLessonDTO
    {
        public string? Title { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string DocumentUrl { get; set; } = null!;
        public int? OrderIndex { get; set; }
    }
}
