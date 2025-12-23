using ECM_BE.Data;
using ECM_BE.Models.DTOs.Course;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ECM_BE.Mappers;

namespace ECM_BE.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AllCourseDTO>> GetAllCourseAsync()
        {
            return await _context.Courses
                .AsNoTracking()
                .Include(c => c.Categories)
                .Include(c => c.Lessons)
                .Include(c => c.Reviews)
                .Select(c => new AllCourseDTO
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    Description = c.Description,
                    ThumbnailUrl = c.ThumbnailUrl,
                    CreatedAt = c.CreatedAt,
                    TotalLessons = c.Lessons != null ? c.Lessons.Count : 0,
                    TotalReviews = c.Reviews != null ? c.Reviews.Count : 0,
                    AverageRating = c.Reviews != null && c.Reviews.Any()
                        ? c.Reviews.Average(r => r.ReviewScore)
                        : (double?)null,
                    Categories = c.Categories.Select(cat => new CategoryDTO
                    {
                        CategoryID = cat.CategoryID,
                        Name = cat.Name,
                        Description = cat.Description
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CourseDTO> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Categories)
                .Include(c => c.Lessons)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                throw new Exception("Course not found");

            return new CourseDTO
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Description = course.Description,
                ThumbnailUrl = course.ThumbnailUrl,
                CreatedAt = course.CreatedAt,
                TotalLessons = course.Lessons?.Count ?? 0,
                TotalReviews = course.Reviews?.Count ?? 0,
                AverageRating = course.Reviews != null && course.Reviews.Any()
                    ? course.Reviews.Average(r => r.ReviewScore)
                    : (double?)null,
                Categories = course.Categories?.Select(cat => cat.Name).ToList()
            };
        }

        public async Task<List<CourseCardDTO>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _context.Courses
                .Where(c => c.Categories.Any(cat => cat.CategoryID == categoryId))
                .Include(c => c.Categories)
                .Include(c => c.Reviews)
                .Select(c => new CourseCardDTO
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Categories = c.Categories.Select(cat => cat.Name).ToList(),
                    TotalReviews = c.Reviews != null ? c.Reviews.Count : 0,
                    AverageRating = c.Reviews != null && c.Reviews.Any()
                        ? c.Reviews.Average(r => r.ReviewScore)
                        : 0
                })
                .ToListAsync();
        }

        private async Task ValidateCategoryRules(List<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0)
            {
                throw new Exception("Category IDs cannot be null or empty");
            }

            if (categoryIds.Count != 2)
            {
                throw new Exception($"A course must have exactly 2 categories: one LEVEL and one SKILL. Received {categoryIds.Count} categories.");
            }

            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.CategoryID))
                .ToListAsync();

            if (categories.Count != 2)
            {
                throw new Exception($"Invalid category IDs provided. Expected 2 valid categories but found {categories.Count}. IDs sent: [{string.Join(", ", categoryIds)}]");
            }

            var descriptions = categories.Select(c => c.Description).Distinct().ToList();

            // Check if we have exactly one LEVEL and one SKILL
            var hasLevel = descriptions.Contains("LEVEL");
            var hasSkill = descriptions.Contains("SKILL");

            if (!hasLevel || !hasSkill)
            {
                var foundDescriptions = string.Join(", ", categories.Select(c => $"{c.Name} ({c.Description})"));
                throw new Exception($"A course must have exactly one LEVEL category and one SKILL category. Found: {foundDescriptions}");
            }

            // Check for duplicates - this should not happen if we already checked LEVEL and SKILL
            if (descriptions.Count != 2)
            {
                throw new Exception("Cannot assign multiple categories with the same description type");
            }
        }

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseRequestDTO requestDto)
        {
            // Validate category rules
            await ValidateCategoryRules(requestDto.CategoryIDs);

            var course = requestDto.ToCourseDTO();

            var categories = await _context.Categories
                .Where(c => requestDto.CategoryIDs.Contains(c.CategoryID))
                .ToListAsync();

            course.Categories = categories;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return course.ToCourseFromCreate();
        }

        public async Task<CourseDTO> UpdateCourseAsync(int courseId, UpdateCourse requestDto)
        {
            var course = await _context.Courses
                .Include(c => c.Categories)
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                throw new Exception("Course not found");

            // Update basic fields
            if (!string.IsNullOrEmpty(requestDto.Title))
                course.Title = requestDto.Title;

            if (!string.IsNullOrEmpty(requestDto.Description))
                course.Description = requestDto.Description;

            if (!string.IsNullOrEmpty(requestDto.ThumbnailUrl))
                course.ThumbnailUrl = requestDto.ThumbnailUrl;

            // Update categories if provided
            if (requestDto.CategoryIDs != null)
            {
                // Validate category rules
                await ValidateCategoryRules(requestDto.CategoryIDs);

                course.Categories.Clear();

                var categories = await _context.Categories
                    .Where(c => requestDto.CategoryIDs.Contains(c.CategoryID))
                    .ToListAsync();

                course.Categories = categories;
            }

            await _context.SaveChangesAsync();

            return course.ToCourseFromCreate();
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                throw new Exception("Course not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}