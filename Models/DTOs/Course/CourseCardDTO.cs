namespace ECM_BE.Models.DTOs.Course
{
    public class CourseCardDTO
    {
        public int CourseID { get; set; }
        public string Title { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public List<string>? Categories { get; set; }     // ví dụ ["IELTS", "Grammar"]
        public double AverageRating { get; set; }          // trung bình sao
        public int TotalReviews { get; set; }              // tổng số đánh giá
    }
}
