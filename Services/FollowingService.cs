using ECM_BE.Models.DTOs.Favorite;
using ECM_BE.Models.Entities;
using ECM_BE.Data;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class FollowingService : IFollowingService
    {
        private readonly AppDbContext _context;
        public FollowingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetFollowingDTO>> GetAllFollowingsCourse(string userId)
        {
            var followings = await _context.Followings
                .Where(f => f.userID == userId)
                .Include(f => f.Course)
                .AsNoTracking()
                .ToListAsync();

            var result = followings.Select(f =>
            {
                var course = f.Course;

                var selectedAttributeDetailIds = new List<int>(); // để trống nếu không có phân loại

                return new GetFollowingDTO
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    ThumbnailUrl = course.ThumbnailUrl ?? string.Empty,
                };
            });

            return result;

        }

        public async Task<bool> ToggleFollowingAsync(string userId, int courseId)
        {
            var existingFollowing = await _context.Followings
                .FirstOrDefaultAsync(f => f.userID == userId && f.CourseID == courseId);

            if (existingFollowing != null)
            {
                _context.Followings.Remove(existingFollowing);
                await _context.SaveChangesAsync();
                return false; // Đã xóa
            }
            else
            {
                var newFollowing = new Following
                {
                    userID = userId,
                    CourseID = courseId,
                    FollowedAt = DateTime.UtcNow
                };
                _context.Followings.Add(newFollowing);
                await _context.SaveChangesAsync();
                return true; // Đã thêm
            }
        }
        public async Task RemoveFollowingAsync(string userId, int courseId)
        {
            var existingFollowing = await _context.Followings
                .FirstOrDefaultAsync(f => f.userID == userId && f.CourseID == courseId);
            if (existingFollowing != null)
            {
                _context.Followings.Remove(existingFollowing);
                await _context.SaveChangesAsync();
            }
        }
}
}
