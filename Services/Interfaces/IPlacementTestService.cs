using ECM_BE.Models.DTOs.PlacementTest;

namespace ECM_BE.Services.Interfaces
{
    public interface IPlacementTestService
    {
        Task<List<AllPlacementTestDTO>> GetAllPlacementTestsAsync();
        Task<PlacementTestDTO> GetPlacementTestByIdAsync(int TestId);
        Task<PlacementTestDTO> CreatePlacementTestAsync(CreatePlacementTestRequestDTO requestDto);
        Task<PlacementTestDTO> UpdatePlacementTestAsync(int TestId, UpdatePlacementTestDTO requestDto);
        Task DeletePlacementTestAsync(int TestId);
    }
}
