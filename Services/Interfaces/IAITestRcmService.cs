using ECM_BE.Models.DTOs.AI;

namespace ECM_BE.Services.Interfaces
{
    public interface IAITestRcmService
    {
        Task<TestRcmDTO?> RecommendTestAsync(string userId);
    }
}
