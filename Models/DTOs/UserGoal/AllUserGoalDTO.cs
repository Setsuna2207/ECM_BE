namespace ECM_BE.Models.DTOs.UserGoal
{
    public class AllUserGoalDTO
    {
        public int UserGoalID { get; set; }
        public string UserID { get; set; } = null!;
        public int CategoryID { get; set; }
        public List<string> CategoryNames { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
