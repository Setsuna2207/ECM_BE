namespace ECM_BE.Models.DTOs.Category
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
