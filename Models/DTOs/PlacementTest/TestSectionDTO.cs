namespace ECM_BE.Models.DTOs.PlacementTest
{
    public class TestSectionDTO
    {
        public int SectionId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Duration { get; set; }
        public List<TestQuestionDTO> Questions { get; set; } = new List<TestQuestionDTO>();
    }
}