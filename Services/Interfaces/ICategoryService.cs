using ECM_BE.Models.DTOs.Category;

namespace ECM_BE.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<AllCategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryRequestDTO requestDto);
        Task<CategoryDTO> UpdateCategoryAsync(int categoryID, UpdateCategoryDTO requestDto);
        Task DeleteCategoryAsync(int categoryID);
    }
}
