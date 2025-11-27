namespace ECM_BE.Models.DTOs.Review
{
    public class ReviewDTO
    {
        public int CourseID { get; set; }
        public string userID { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int ReviewScore { get; set; }
        public string? ReviewContent { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
