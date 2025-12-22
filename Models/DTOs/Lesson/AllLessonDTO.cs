namespace ECM_BE.Models.DTOs.Lesson
{
    public class AllLessonDTO
    {
        public int LessonID { get; set; }
        public int CourseID { get; set; }
        public string? Title { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string DocumentUrl { get; set; } = null!;
        public int? OrderIndex { get; set; }

    }
}
