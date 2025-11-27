using ECM_BE.Models.DTOs.Category;
using ECM_BE.Models.Entities;

namespace ECM_BE.Models.Mapper
{
    public static class CategoryMapper
    {
        public static CategoryDTO ToCategoryDto(this Category category)
        {
            return new CategoryDTO
            {
                CategoryID = category.CategoryID,
                Name = category.Name,
                Description = category.Description,
            };
        }
        public static Category CategoryFromCreate(this CreateCategoryRequestDTO requestDto)
        {
            return new Category
            {
                Name = requestDto.Name,
                Description = requestDto.Description,
            };
        }
        public static void UpdateCategoryFromDto(this Category category, UpdateCategoryDTO requestDto)
        {
            category.Name = requestDto.Name;
            category.Description = requestDto.Description;
        }
    }
}
