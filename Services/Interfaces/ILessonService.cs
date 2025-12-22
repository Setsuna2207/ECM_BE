using ECM_BE.Models.DTOs.Lesson;

namespace ECM_BE.Services.Interfaces
{
    public interface ILessonService
    {
        Task<List<AllLessonDTO>> GetAllLessonsAsync();
        Task<LessonDTO> GetLessonByCourseIdAsync(int courseId);
        Task<LessonDTO> GetLessonByIdAsync(int lessonId);
        Task<LessonDTO> CreateLessonAsync(CreateLessonRequestDTO requestDto);
        Task<LessonDTO> UpdateLessonAsync(int lessonId, UpdateLessonDTO requestDto);
        Task DeleteLessonAsync(int lessonId);
    }
}
