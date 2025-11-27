using ECM_BE.Models.DTOs.Category;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService CategoryService)
        {
            _categoryService = CategoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var Categorys = await _categoryService.GetAllCategoriesAsync();
            return Ok(Categorys);
        }
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _categoryService.CreateCategoryAsync(requestDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{CategoryId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateCategory(int CategoryId, [FromBody] UpdateCategoryDTO requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(CategoryId, requestDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpDelete("{CategoryId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(CategoryId);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
