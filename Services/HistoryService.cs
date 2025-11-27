using ECM_BE.Data;
using ECM_BE.Exceptions.Custom;
using ECM_BE.Models.DTOs.History;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly AppDbContext _context;

        public HistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<History> CreateHistoryAsync(string userId, int courseId)
        {
            var existedCourse = await _context.Courses.FirstOrDefaultAsync(p => p.CourseID == courseId);
            if (existedCourse == null)
            {
                throw new BadRequestException("Không tìm thấy khóa học");
            }

            var existedHistory = await _context.Histories
                .FirstOrDefaultAsync(h => h.userID == userId && h.CourseID == courseId);

            if (existedHistory != null)
            {
                existedHistory.LastAccessed = DateTime.Now;
                _context.Histories.Update(existedHistory);
                await _context.SaveChangesAsync();
                return existedHistory;
            }

            var newHistory = new History
            {
                userID = userId,
                CourseID = courseId,
                LastAccessed = DateTime.Now
            };
            await _context.Histories.AddAsync(newHistory);
            await _context.SaveChangesAsync();
            return newHistory;
        }

        public async Task DeleteAllHistoriesAsync(string userId)
        {
            try
            {
                var hitories = await _context.Histories
                    .Where(h => h.userID == userId)
                    .ToListAsync();
                _context.Histories.RemoveRange(hitories);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DeleteHistoryAsync(string userId, int courseId)
        {
            try
            {
                var existedCourse = await _context.Courses
                    .FirstOrDefaultAsync(p => p.CourseID == courseId);
                if (existedCourse == null)
                {
                    throw new BadRequestException("Không tìm thấy khóa học");
                }
                var existedHistory = await _context.Histories
                    .FirstOrDefaultAsync(h => h.userID == userId && h.CourseID == courseId);
                if (existedHistory == null)
                {
                    throw new NotFoundException("Không tìm thấy bản ghi");
                }
                _context.Histories.Remove(existedHistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<HistoryDTO>> GetHistoriesAsync(string userId)
        {
            try
            {
                var histories = await _context.Histories
                    .AsNoTracking()
                    .Where(h => h.userID == userId)
                    .Include(h => h.Course)
                    .Select(h => new HistoryDTO
                    {
                        HistoryID = h.HistoryID,
                        UserID = h.userID,
                        CourseID = h.CourseID,
                        Progress = h.Progress,
                        LastAccessed = h.LastAccessed,

                        CourseTitle = h.Course.Title,
                        ThumbnailUrl = h.Course.ThumbnailUrl,

                        TotalLessons = h.Course.Lessons.Count,
                        CompletedLessons = _context.Lessons
                        .Count(l => l.CourseID == h.CourseID &&
                                    l.Quizzes.Any(q => _context.QuizResults
                                        .Any(qr => qr.userID == userId && qr.QuizID == q.QuizID)))
                    })
                    .OrderByDescending(h => h.LastAccessed)
                    .ToListAsync();

                return histories;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<HistoryDTO> UpdateHistoryAsync(string userId, int courseID)
        {
            try
            {
                var existedCourse = await _context.Courses
                    .FirstOrDefaultAsync(p => p.CourseID == courseID);
                if (existedCourse == null)
                {
                    throw new BadRequestException("Không tìm thấy khóa học");
                }
                var existedHistory = await _context.Histories
                    .FirstOrDefaultAsync(h => h.userID == userId && h.CourseID == courseID);
                if (existedHistory == null)
                {
                    throw new NotFoundException("Không tìm thấy bản ghi");
                }
                existedHistory.LastAccessed = DateTime.Now;
                await _context.SaveChangesAsync();
                return existedHistory.ToHistoryDTO();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HistoryDTO> UpdateProgressAsync(string userId, int courseID)
        {
            try
            {
                // Lấy khóa học và danh sách bài học
                var course = await _context.Courses
                    .Include(c => c.Lessons)
                    .ThenInclude(l => l.Quizzes)
                    .FirstOrDefaultAsync(c => c.CourseID == courseID);

                if (course == null)
                    throw new BadRequestException("Không tìm thấy khóa học");

                var totalLessons = course.Lessons.Count;
                if (totalLessons == 0)
                    throw new BadRequestException("Khóa học chưa có bài giảng");

                // Lấy danh sách bài học mà user đã hoàn thành quiz
                // Một bài học được coi là hoàn thành nếu TẤT CẢ quiz trong lesson có kết quả đúng (IsCorrect = true)
                var completedLessonIds = new List<int>();

                foreach (var lesson in course.Lessons)
                {
                    bool allQuizPassed = true;

                    foreach (var quiz in lesson.Quizzes)
                    {
                        // Kiểm tra user đã làm bài quiz chưa
                        var hasPassed = await _context.QuizResults
                            .AnyAsync(qr => qr.userID == userId &&
                                            qr.QuizID == quiz.QuizID);


                        if (!hasPassed)
                        {
                            allQuizPassed = false;
                            break;
                        }
                    }

                    if (allQuizPassed)
                        completedLessonIds.Add(lesson.LessonID);
                }

                // Tính tiến độ (%)
                float progress = (float)completedLessonIds.Count / totalLessons * 100f;

                // Cập nhật vào bảng History
                var history = await _context.Histories
                    .FirstOrDefaultAsync(h => h.userID == userId && h.CourseID == courseID);

                if (history == null)
                {
                    history = new History
                    {
                        userID = userId,
                        CourseID = courseID,
                        Progress = progress,
                        LastAccessed = DateTime.Now
                    };
                    await _context.Histories.AddAsync(history);
                }
                else
                {
                    history.Progress = progress;
                    history.LastAccessed = DateTime.Now;
                    _context.Histories.Update(history);
                }

                await _context.SaveChangesAsync();

                return history.ToHistoryDTO();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
