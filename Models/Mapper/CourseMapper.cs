using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.Course;

namespace ECM_BE.Mappers
{
    public static class CourseMapper
    {
        public static CourseDTO ToCourseFromCreate(this Course course)
        {
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
        public static Course ToCourseDTO(this CreateCourseRequestDTO dto)
        {
            return new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                ThumbnailUrl = dto.ThumbnailUrl,
                CreatedAt = DateTime.UtcNow
            };
        }
        public static void UpdateCourseFromDTO(this Course course, UpdateCourse dto)
        {
            course.Title = dto.Title;
            course.Description = dto.Description;
            course.ThumbnailUrl = dto.ThumbnailUrl;
        }
    }
}
