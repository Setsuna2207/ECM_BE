namespace ECM_BE.Models.DTOs.Favorite
{
    public class GetFollowingDTO
    {
        public int CourseID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;

    }
}
