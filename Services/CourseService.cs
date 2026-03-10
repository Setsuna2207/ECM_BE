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
                    TotalLessons = c.Lessons.Count,
                    TotalReviews = c.Reviews.Count,
                    AverageRating = c.Reviews.Any()
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
                .AsNoTracking()
                .Where(c => c.CourseID == courseId)
                .Select(c => new CourseDTO
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    Description = c.Description,
                    ThumbnailUrl = c.ThumbnailUrl,
                    CreatedAt = c.CreatedAt,
                    TotalLessons = c.Lessons.Count,
                    TotalReviews = c.Reviews.Count,
                    AverageRating = c.Reviews.Any()
                        ? c.Reviews.Average(r => r.ReviewScore)
                        : (double?)null,
                    Categories = c.Categories.Select(cat => cat.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null)
                throw new Exception("Course not found");

            return course;
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
            Console.WriteLine($"[ValidateCategoryRules] Validating category IDs: {string.Join(", ", categoryIds ?? new List<int>())}");
            
            if (categoryIds == null || categoryIds.Count == 0)
            {
                Console.WriteLine($"[ValidateCategoryRules] ERROR: Category IDs are null or empty");
                throw new Exception("Category IDs cannot be null or empty");
            }

            if (categoryIds.Count != 2)
            {
                Console.WriteLine($"[ValidateCategoryRules] ERROR: Expected 2 categories, got {categoryIds.Count}");
                throw new Exception($"A course must have exactly 2 categories: one LEVEL and one SKILL. Received {categoryIds.Count} categories.");
            }

            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.CategoryID))
                .ToListAsync();

            Console.WriteLine($"[ValidateCategoryRules] Found {categories.Count} categories in database");
            foreach (var cat in categories)
            {
                Console.WriteLine($"[ValidateCategoryRules]   - ID: {cat.CategoryID}, Name: {cat.Name}, Description: {cat.Description}");
            }

            if (categories.Count != 2)
            {
                Console.WriteLine($"[ValidateCategoryRules] ERROR: Invalid category IDs");
                throw new Exception($"Invalid category IDs provided. Expected 2 valid categories but found {categories.Count}. IDs sent: [{string.Join(", ", categoryIds)}]");
            }

            var descriptions = categories.Select(c => c.Description).Distinct().ToList();

            // Check if we have exactly one LEVEL and one SKILL
            var hasLevel = descriptions.Contains("LEVEL");
            var hasSkill = descriptions.Contains("SKILL");

            Console.WriteLine($"[ValidateCategoryRules] Has LEVEL: {hasLevel}, Has SKILL: {hasSkill}");

            if (!hasLevel || !hasSkill)
            {
                var foundDescriptions = string.Join(", ", categories.Select(c => $"{c.Name} ({c.Description})"));
                Console.WriteLine($"[ValidateCategoryRules] ERROR: Missing LEVEL or SKILL category");
                throw new Exception($"A course must have exactly one LEVEL category and one SKILL category. Found: {foundDescriptions}");
            }

            // Check for duplicates - this should not happen if we already checked LEVEL and SKILL
            if (descriptions.Count != 2)
            {
                Console.WriteLine($"[ValidateCategoryRules] ERROR: Duplicate category types");
                throw new Exception("Cannot assign multiple categories with the same description type");
            }
            
            Console.WriteLine($"[ValidateCategoryRules] Validation passed!");
        }

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseRequestDTO requestDto)
        {
            Console.WriteLine($"[CreateCourse] Starting course creation");
            Console.WriteLine($"[CreateCourse] Title: {requestDto.Title}");
            Console.WriteLine($"[CreateCourse] CategoryIDs: {string.Join(", ", requestDto.CategoryIDs ?? new List<int>())}");
            
            // Validate category rules
            await ValidateCategoryRules(requestDto.CategoryIDs);

            var course = requestDto.ToCourseDTO();

            // Fetch categories with AsNoTracking to avoid tracking issues
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => requestDto.CategoryIDs.Contains(c.CategoryID))
                .ToListAsync();

            Console.WriteLine($"[CreateCourse] Found {categories.Count} categories: {string.Join(", ", categories.Select(c => $"{c.Name} ({c.Description})"))}");

            // Add the course first WITHOUT categories
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[CreateCourse] Course created with ID: {course.CourseID}");

            // Now attach the existing categories to the course
            // This tells EF that these categories already exist in the database
            foreach (var categoryId in requestDto.CategoryIDs)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    course.Categories.Add(category);
                }
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"[CreateCourse] Categories linked successfully");

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