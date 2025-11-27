using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.Course
{
    public class CreateCourseRequestDTO
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;

        // Gắn nhiều category cho khóa học (ví dụ: [Ielts, Listening])
        public List<int>? CategoryIDs { get; set; }

        // Tuỳ chọn (backend có thể tự set nếu không gửi)
        public DateTime? CreatedAt { get; set; }
    }
}
