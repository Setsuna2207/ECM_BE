using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.Review
{
    public class UpdateReviewDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5.")]
        public int ReviewScore { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Nội dung đánh giá không được vượt quá 1000 ký tự.")]
        public string? ReviewContent { get; set; }
    }
}
