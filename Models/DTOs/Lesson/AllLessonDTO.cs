namespace ECM_BE.Models.DTOs.Lesson
{
    public class AllLessonDTO
    {
        public int LessonID { get; set; }
        public int CourseID { get; set; }
        public string? Title { get; set; }
        public string VideoUrl { get; set; } = null!;
        public List<string> DocumentUrl { get; set; } = new List<string>();
        public int? OrderIndex { get; set; }

    }
}
