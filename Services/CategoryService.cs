using ECM_BE.Data;
using ECM_BE.Exceptions.Custom;
using ECM_BE.Models.DTOs.Category;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AllCategoryDTO>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(p => new AllCategoryDTO
                {
                    CategoryID = p.CategoryID,
                    Name = p.Name,
                    Description = p.Description,
                })
                .ToListAsync();
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryRequestDTO requestDto)
        {
            try
            {
                var category = requestDto.CategoryFromCreate(); 
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return category.ToCategoryDto();                 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int categoryID, UpdateCategoryDTO requestDto)
        {
            var category = await _context.Categories.FindAsync(categoryID);
            if (category == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục.");
            }

            category.UpdateCategoryFromDto(requestDto);
            await _context.SaveChangesAsync();

            return category.ToCategoryDto();
        }

        public async Task DeleteCategoryAsync(int categoryID)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(r => r.CategoryID == categoryID);
            if (category == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

    }
}
