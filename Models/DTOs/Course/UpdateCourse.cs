using Newtonsoft.Json;

namespace ECM_BE.Models.DTOs.Course
{
    public class UpdateCourse
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }

        // Cập nhật lại danh sách Category
        [JsonProperty("categoryIDs")]
        public List<int>? CategoryIDs { get; set; }
    }
}
