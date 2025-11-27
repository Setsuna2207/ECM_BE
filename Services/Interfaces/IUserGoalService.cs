using ECM_BE.Models.DTOs.UserGoal;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Services.Interfaces
{
    public interface IUserGoalService
    {
        Task<List<UserGoalDTO>> GetUserGoalsAsync(string userID);
        Task<UserGoalDTO> CreateUserGoalAsync(string userID, CreateUserGoalRequestDTO requestDto);
        Task<UserGoalDTO> UpdateUserGoalAsync(string userID, int userGoalID, UpdateUserGoalDTO requestDto);
        Task DeleteUserGoalAsync(string userID, int userGoalID);
    }
}
