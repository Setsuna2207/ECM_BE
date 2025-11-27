using ECM_BE.Models.DTOs.Course;

namespace ECM_BE.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<AllCourseDTO>> GetAllCourseAsync();
        Task<CourseDTO> GetCourseByIdAsync(int courseId);
        Task<List<CourseCardDTO>> GetCoursesByCategoryAsync(int categoryId);
        Task<CourseDTO> CreateCourseAsync(CreateCourseRequestDTO requestDto);
        Task<CourseDTO> UpdateCourseAsync(int courseId, UpdateCourse requestDto);
        Task DeleteCourseAsync(int courseId);
    }
}
