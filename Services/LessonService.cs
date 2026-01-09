using ECM_BE.Data;
using ECM_BE.Models.DTOs.Lesson;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                    OrderIndex = l.OrderIndex,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync()
                .ContinueWith(task => task.Result.Select(dto => new AllLessonDTO
                {
                    LessonID = dto.LessonID,
                    CourseID = dto.CourseID,
                    Title = dto.Title,
                    OrderIndex = dto.OrderIndex,
                    CreatedAt = dto.CreatedAt
                }).ToList());
        }

        public async Task<List<LessonDTO>> GetLessonByCourseIdAsync(int courseId)
        {
            var lessons = await _context.Lessons
                .AsNoTracking()
                .Where(l => l.CourseID == courseId)
                .OrderBy(l => l.OrderIndex)
                .Select(l => new LessonDTO
                {
                    LessonID = l.LessonID,
                    CourseID = l.CourseID,
                    Title = l.Title,
                    OrderIndex = l.OrderIndex,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            if (!lessons.Any())
                throw new Exception("No lessons found for this course");

            return lessons;
        }

        public async Task<LessonDTO> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await _context.Lessons
                .AsNoTracking()
                .Where(l => l.LessonID == lessonId)
                .Select(l => new LessonDTO
                {
                    LessonID = l.LessonID,
                    CourseID = l.CourseID,
                    Title = l.Title,
                    OrderIndex = l.OrderIndex,
                    CreatedAt = l.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (lesson == null)
                throw new Exception("Lesson not found");

            return lesson;
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

            lesson.UpdateFromDto(requestDto);

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