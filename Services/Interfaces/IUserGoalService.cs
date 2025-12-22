using ECM_BE.Models.DTOs.UserGoal;

namespace ECM_BE.Services.Interfaces
{
    public interface IUserGoalService
    {
        Task<List<AllUserGoalDTO>> GetAllUserGoalsAsync();
        Task<UserGoalDTO> GetUserGoalByIdAsync(int userGoalId);
        Task<UserGoalDTO> CreateUserGoalAsync(CreateUserGoalRequestDTO requestDto);
        Task<UserGoalDTO> UpdateUserGoalAsync(int userGoalId, UpdateUserGoalDTO requestDto);
        Task DeleteUserGoalAsync(int userGoalId);
    }
}
