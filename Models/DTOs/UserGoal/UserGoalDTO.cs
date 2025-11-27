namespace ECM_BE.Models.DTOs.UserGoal
{
    public class UserGoalDTO
    {
        public int UserGoalID { get; set; }
        public string UserID { get; set; } = null!;
        public int CategoryID { get; set; }
        public List<string> Name { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
