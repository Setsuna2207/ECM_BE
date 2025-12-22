using ECM_BE.Data;
using ECM_BE.Models.DTOs.Lesson;
using ECM_BE.Models.Entities;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ECM_BE.Models.Mapper;

namespace ECM_BE.Services
{
    public class LessonService : ILessonService
    {
        private readonly AppDbContext _context;

        public LessonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AllLessonDTO>> GetAllLessonsAsync()
        {
            return await _context.Lessons
                .AsNoTracking()
                .Select(l => new AllLessonDTO
                {
                    LessonID = l.LessonID,
                    CourseID = l.CourseID,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    DocumentUrl = l.DocumentUrl,
                    OrderIndex = l.OrderIndex
                })
                .ToListAsync();
        }

        public async Task<LessonDTO> GetLessonByCourseIdAsync(int courseId)
        {
            var lesson = await _context.Lessons
                .Where(l => l.CourseID == courseId)
                .OrderBy(l => l.OrderIndex)
                .FirstOrDefaultAsync();

            if (lesson == null)
                throw new Exception("Lesson not found");

            return lesson.ToLessonDto();
        }

        public async Task<LessonDTO> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.LessonID == lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            return lesson.ToLessonDto();
        }

        public async Task<LessonDTO> CreateLessonAsync(CreateLessonRequestDTO requestDto)
        {
            var lesson = requestDto.ToLessonFromCreate(); 

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return lesson.ToLessonDto(); 
        }

        public async Task<LessonDTO> UpdateLessonAsync(int lessonId, UpdateLessonDTO requestDto)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.LessonID == lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            lesson.Title = requestDto.Title;
            lesson.VideoUrl = requestDto.VideoUrl;
            lesson.DocumentUrl = requestDto.DocumentUrl;
            lesson.OrderIndex = requestDto.OrderIndex;

            await _context.SaveChangesAsync();

            return lesson.ToLessonDto();
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.LessonID == lessonId);

            if (lesson == null)
                throw new Exception("Lesson not found");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}
