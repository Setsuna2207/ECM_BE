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
                .Select(c => new AllCourseDTO
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    Description = c.Description,
                    ThumbnailUrl = c.ThumbnailUrl,
                    CreatedAt = c.CreatedAt
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
                TotalReviews = course.Reviews?.Count ?? 0
            };
        }

        public async Task<List<CourseCardDTO>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _context.Courses
                .Where(c => c.Categories.Any(cat => cat.CategoryID == categoryId))
                .Select(c => new CourseCardDTO
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    ThumbnailUrl = c.ThumbnailUrl,
                })
                .ToListAsync();
        }

        public async Task<CourseDTO> CreateCourseAsync(CreateCourseRequestDTO requestDto)
        {
            var course = requestDto.ToCourseDTO(); // dùng mapper

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return course.ToCourseFromCreate(); // dùng mapper
        }

        public async Task<CourseDTO> UpdateCourseAsync(int courseId, UpdateCourse requestDto)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                throw new Exception("Course not found");

            course.UpdateCourseFromDTO(requestDto); // mapper update

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
