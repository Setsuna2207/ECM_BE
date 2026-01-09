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
            var result = await _context.Followings
                .AsNoTracking()
                .Where(f => f.userID == userId)
                .Select(f => new GetFollowingDTO
                {
                    CourseID = f.Course.CourseID,
                    Title = f.Course.Title,
                    ThumbnailUrl = f.Course.ThumbnailUrl ?? string.Empty,
                })
                .ToListAsync();

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
                return false;
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
                return true;
            }
        }

        public async Task RemoveFollowingAsync(string userId, int courseId)
        {
            await _context.Followings
                .Where(f => f.userID == userId && f.CourseID == courseId)
                .ExecuteDeleteAsync();
        }
    }
}
