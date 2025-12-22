using ECM_BE.Models.DTOs.AIFeedback;

namespace ECM_BE.Services.Interfaces
{
    public interface IAIFeedbackService
    {
        Task<List<AllAIFeedbackDTO>> GetAllAIFeedbacksAsync();
        Task<AIFeedbackDTO> GetAIFeedbackByIdAsync(int aiFeedbackId);
        Task<AIFeedbackDTO> CreateAIFeedbackAsync(CreateAIFeedbackRequestDTO requestDto);
        Task<AIFeedbackDTO> UpdateAIFeedbackAsync(int aiFeedbackId, UpdateAIFeedbackDTO requestDto);
        Task DeleteAIFeedbackAsync(int aiFeedbackId);
    }
}
