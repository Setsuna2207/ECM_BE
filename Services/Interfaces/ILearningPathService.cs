using ECM_BE.Models.DTOs.LearningPath;

namespace ECM_BE.Services.Interfaces
{
    public interface ILearningPathService
    {
        Task<LearningPathDTO?> GetActiveLearningPathByUserIdAsync(string userId);
        Task<LearningPathDTO> GetLearningPathByIdAsync(int learningPathId);
        Task<LearningPathDTO> UpdateLearningPathAsync(int learningPathId, UpdateLearningPathDTO dto);
    }
}
