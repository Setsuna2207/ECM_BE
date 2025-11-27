using System.ComponentModel.DataAnnotations;

namespace ECM_BE.Models.DTOs.Category
{
    public class CreateCategoryRequestDTO
    {
        [StringLength(20)]
        [Required]
        public string Name { get; set; } = null!;
        [StringLength(255)]
        [Required]
        public string? Description { get; set; }
    }
}
