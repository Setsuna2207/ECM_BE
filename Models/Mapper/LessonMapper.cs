using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.Lesson;
using System.Text.Json;

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
                DocumentUrl = SerializeDocumentUrls(requestDto.DocumentUrl),
                OrderIndex = requestDto.OrderIndex,
            };
        }

        public static LessonDTO ToLessonDto(this Lesson lesson)
        {
            return new LessonDTO
            {
                LessonID = lesson.LessonID,
                CourseID = lesson.CourseID,
                Title = lesson.Title,
                VideoUrl = lesson.VideoUrl,
                DocumentUrl = DeserializeDocumentUrls(lesson.DocumentUrl),
                OrderIndex = lesson.OrderIndex,
            };
        }

        public static AllLessonDTO ToAllLessonDto(this Lesson lesson)
        {
            return new AllLessonDTO
            {
                LessonID = lesson.LessonID,
                CourseID = lesson.CourseID,
                Title = lesson.Title,
                VideoUrl = lesson.VideoUrl,
                DocumentUrl = DeserializeDocumentUrls(lesson.DocumentUrl),
                OrderIndex = lesson.OrderIndex,
            };
        }

        public static void UpdateFromDto(this Lesson lesson, UpdateLessonDTO requestDto)
        {
            lesson.Title = requestDto.Title;
            lesson.VideoUrl = requestDto.VideoUrl;
            lesson.DocumentUrl = SerializeDocumentUrls(requestDto.DocumentUrl);
            lesson.OrderIndex = requestDto.OrderIndex;
        }

        private static string SerializeDocumentUrls(List<string> urls)
        {
            return JsonSerializer.Serialize(urls);
        }

        private static List<string> DeserializeDocumentUrls(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}