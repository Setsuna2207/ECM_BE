using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.UserGoal
{
    public class CreateUserGoalRequestDTO
    {
        public string? UserID { get; set; }
        
        [Required(ErrorMessage = "Content is required")]
        [MinLength(1, ErrorMessage = "Content cannot be empty")]
        public string? Content { get; set; }
    }
}
