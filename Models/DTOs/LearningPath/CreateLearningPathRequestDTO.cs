using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.LearningPath
{
    public class CreateLearningPathRequestDTO
    {
        public string? UserID { get; set; }
        
        [Required(ErrorMessage = "UserGoalID is required")]
        public int UserGoalID { get; set; }
    }
}
