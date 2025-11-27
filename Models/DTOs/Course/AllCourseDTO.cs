namespace ECM_BE.Models.DTOs.Course
{
    public class AllCourseDTO
    {
        public int CourseID { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Thông tin phụ (tính từ quan hệ)
        public int TotalLessons { get; set; }
        public int TotalReviews { get; set; }
        public double? AverageRating { get; set; }

        // Danh mục khóa học
        public List<string>? Categories { get; set; }
    }
}
