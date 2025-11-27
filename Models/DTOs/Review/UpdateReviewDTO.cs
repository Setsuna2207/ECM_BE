using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.Review
{
    public class UpdateReviewDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5.")]
        public int ReviewScore { get; set; }
        public string? ReviewContent { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
