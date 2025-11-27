using ECM_BE.Models.DTOs.History;
using ECM_BE.Models.Entities;

namespace ECM_BE.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<History> CreateHistoryAsync(string userId, int courseID);
        Task<ICollection<HistoryDTO>> GetHistoriesAsync(string userId);
        Task<HistoryDTO> UpdateHistoryAsync(string userId, int courseID);
        Task<HistoryDTO> UpdateProgressAsync(string userId, int courseID);
        Task DeleteHistoryAsync(string userId, int courseID);
        Task DeleteAllHistoriesAsync(string userId);
    }
}
