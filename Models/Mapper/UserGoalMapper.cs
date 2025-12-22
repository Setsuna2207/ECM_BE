using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.UserGoal;

namespace ECM_BE.Models.Mapper
{
    public static class UserGoalMapper
    {
        public static UserGoal ToUserGoalFromCreate(this CreateUserGoalRequestDTO requestDto, string userID)
        {
            return new UserGoal
            {
                Content = requestDto.Content,
            };
        }
        public static UserGoalDTO ToUserGoalDto(this UserGoal requestDto)
        {
            return new UserGoalDTO
            {
                UserGoalID = requestDto.UserGoalID,
                UserID = requestDto.userID,
                Content = requestDto.Content,
            };
        }
    }
}
