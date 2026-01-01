using ECM_BE.Models.DTOs.AI;

namespace ECM_BE.Services.Interfaces
{
    public interface IAICourseRcmService
    {
        Task<CourseRcmDTO?> RecommendCourseAsync(string userId);
    }
}
