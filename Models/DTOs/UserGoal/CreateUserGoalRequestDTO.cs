using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.UserGoal
{
    public class CreateUserGoalRequestDTO
    {
        [Required]
        public List<int> CategoryIDs { get; set; } = new();
    }
}
