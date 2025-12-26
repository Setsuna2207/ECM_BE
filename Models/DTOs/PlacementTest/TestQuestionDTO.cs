namespace ECM_BE.Models.DTOs.PlacementTest
{
    public class TestQuestionDTO
    {
        public int QuestionId { get; set; }
        public string Type { get; set; } = null!; // "multiple-choice", "essay", "short-response", etc.
        public string Question { get; set; } = null!;
        public string? Passage { get; set; }
        public List<string>? Options { get; set; }
        public int? CorrectAnswer { get; set; }
        public string? CorrectAnswerText { get; set; }
        public int Points { get; set; }
        public int? MinWords { get; set; }
        public int? MaxWords { get; set; }
        public int? ExpectedWords { get; set; }
        public string? SampleAnswer { get; set; }
        public Dictionary<string, int>? Rubric { get; set; }
    }
}