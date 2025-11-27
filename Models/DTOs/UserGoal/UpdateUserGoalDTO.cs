using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.UserGoal
{
    public class UpdateUserGoalDTO
    {
        [Required]
        public int UserGoalID { get; set; }
        public List<int> CategoryIDs { get; set; } = new();
    }
}
