namespace ECM_BE.Models.DTOs.AIRecommendation
{
    public class GoalAnalysisRequestDTO
    {
        // UserID will be set from authenticated user
        // No need for GoalContent - AI will read from UserGoal table
        public string? UserID { get; set; }
    }
}
