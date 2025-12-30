namespace ECM_BE.Models.DTOs.AIRecommendation
{
    public class GoalAnalysisResponseDTO
    {
        public int LearningPathID { get; set; }
        public string ParsedGoal { get; set; } = null!;
        public string Category { get; set; } = null!; // TOEIC, IELTS, TOEFL, GENERAL
        public string Skill { get; set; } = null!; // READING, LISTENING, WRITING, SPEAKING, ALL
        public int? TargetScore { get; set; }
        public string Level { get; set; } = null!; // Beginner, Intermediate, Advanced
        public int RecommendedTestID { get; set; }
        public string RecommendedTestTitle { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
