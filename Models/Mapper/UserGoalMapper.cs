using ECM_BE.Models.DTOs.UserGoal;
using ECM_BE.Models.Entities;

namespace ECM_BE.Models.Mapper
{
    public static class UserGoalMapper
    {
        public static UserGoalDTO ToUserGoalDTO(this UserGoal goal)
        {
            return new UserGoalDTO
            {
                UserGoalID = goal.UserGoalID,
                UserID = goal.userID,
                CategoryID = goal.CategoryID,
                Name = goal.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                UpdatedAt = goal.UpdatedAt
            };
        }
    }
}
