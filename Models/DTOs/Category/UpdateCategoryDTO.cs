using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.Category
{
    public class UpdateCategoryDTO
    {
        [StringLength(20)]
        public string Name { get; set; } = null!;
        [StringLength(255)]
        public string? Description { get; set; }
    }
}
