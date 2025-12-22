using ECM_BE.Models.DTOs.Favorite;

namespace ECM_BE.Services.Interfaces
{
    public interface IFollowingService
    {
        Task<IEnumerable<GetFollowingDTO>> GetAllFollowingsCourse(string userId);
        Task<bool> ToggleFollowingAsync(string userId, int CourseID);
        Task RemoveFollowingAsync(string userId, int CourseID);
    }
}
