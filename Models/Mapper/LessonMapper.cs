using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.Lesson;

namespace ECM_BE.Models.Mapper
{
    public static class LessonMapper
    {
        public static Lesson ToLessonFromCreate(this CreateLessonRequestDTO requestDto)
        {
            return new Lesson
            {
                CourseID = requestDto.CourseID,
                Title = requestDto.Title,
                VideoUrl = requestDto.VideoUrl,
                DocumentUrl = requestDto.DocumentUrl,
                OrderIndex = requestDto.OrderIndex,
            };
        }
        public static LessonDTO ToLessonDto(this Lesson requestDto)
        {
            return new LessonDTO
            {
                CourseID = requestDto.CourseID,
                LessonID = requestDto.LessonID,
                Title = requestDto.Title,
                VideoUrl = requestDto.VideoUrl,
                DocumentUrl = requestDto.DocumentUrl,
                OrderIndex = requestDto.OrderIndex,
            };
        }
    }
}
