namespace ECM_BE.Models.DTOs.PlacementTest
{
    public class AllPlacementTestDTO
    {
        public int TestID { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public int TotalQuestions { get; set; }
        public string QuestionFileURL { get; set; } = null!;
        public string MediaURL { get; set; } = null!;
        public string? Category { get; set; }
        public string? Level { get; set; }
        public List<TestSectionDTO>? Sections { get; set; }
    }
}
