using ECM_BE.Data;
using ECM_BE.Exceptions.Custom;
using ECM_BE.Models.DTOs.UserGoal;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class UserGoalService : IUserGoalService
    {
        private readonly AppDbContext _context;

        public UserGoalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserGoalDTO>> GetUserGoalsAsync(string userId)
        {
            var goals = await _context.UserGoals
                .AsNoTracking()
                .Where(ug => ug.userID == userId)
                .Include(ug => ug.Categories)
                .Select(ug => new UserGoalDTO
                {
                    UserGoalID = ug.UserGoalID,
                    UserID = ug.userID,
                    CategoryID = ug.CategoryID,
                    Name = ug.Categories.Select(c => c.Name).ToList(),
                    UpdatedAt = ug.UpdatedAt
                })
                .ToListAsync();

            return goals;
        }

        public async Task<UserGoalDTO> CreateUserGoalAsync(string userId, CreateUserGoalRequestDTO requestDto)
        {
            if (requestDto.CategoryIDs == null || !requestDto.CategoryIDs.Any())
                throw new BadRequestException("Cần chọn ít nhất một danh mục cho mục tiêu học.");

            // Kiểm tra trùng tên mục tiêu cho user
            bool existedGoal = await _context.UserGoals.AnyAsync(ug => ug.userID == userId);
            if (existedGoal)
                throw new ConflictException("Tên mục tiêu học đã tồn tại.");

            var categories = await _context.Categories
                .Where(c => requestDto.CategoryIDs.Contains(c.CategoryID))
                .ToListAsync();

            if (categories.Count != requestDto.CategoryIDs.Count)
                throw new BadRequestException("Một hoặc nhiều danh mục không tồn tại.");

            var goal = new UserGoal
            {
                userID = userId,
                UpdatedAt = DateTime.UtcNow,
                Categories = categories
            };

            await _context.UserGoals.AddAsync(goal);
            await _context.SaveChangesAsync();

            return new UserGoalDTO
            {
                UserGoalID = goal.UserGoalID,
                UserID = goal.userID,
                CategoryID = goal.CategoryID,
                Name = goal.Categories.Select(c => c.Name).ToList(),
                UpdatedAt = goal.UpdatedAt
            };
        }

        public async Task<UserGoalDTO> UpdateUserGoalAsync(string userID, int userGoalID, UpdateUserGoalDTO requestDto)
        {
            var goal = await _context.UserGoals
                .Include(g => g.Categories)
                .FirstOrDefaultAsync(g => g.UserGoalID == requestDto.UserGoalID && g.userID == userID);

            if (goal == null)
                throw new NotFoundException("Không tìm thấy mục tiêu học.");

            // Cập nhật danh sách category
            if (requestDto.CategoryIDs != null && requestDto.CategoryIDs.Any())
            {
                var categories = await _context.Categories
                    .Where(c => requestDto.CategoryIDs.Contains(c.CategoryID))
                    .ToListAsync();

                if (categories.Count != requestDto.CategoryIDs.Count)
                    throw new BadRequestException("Một hoặc nhiều danh mục không tồn tại.");

                goal.Categories.Clear(); // Xóa liên kết cũ
                foreach (var cat in categories)
                    goal.Categories.Add(cat);
            }
            goal.UpdatedAt = DateTime.UtcNow;

            _context.UserGoals.Update(goal);
            await _context.SaveChangesAsync();

            return goal.ToUserGoalDTO();
        }

        public async Task DeleteUserGoalAsync(string userID, int userGoalId)
        {
            var goal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserGoalID == userGoalId && g.userID == userID);

            if (goal == null)
                throw new NotFoundException("Không tìm thấy bản ghi mục tiêu học.");

            _context.UserGoals.Remove(goal);
            await _context.SaveChangesAsync();
        }
    }
}
