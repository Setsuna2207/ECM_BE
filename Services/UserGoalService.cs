using ECM_BE.Data;
using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.UserGoal;
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
        public async Task<List<AllUserGoalDTO>> GetAllUserGoalsAsync()
        {
            return await _context.UserGoals
                .Select(x => new AllUserGoalDTO
                {
                    UserGoalID = x.UserGoalID,
                    UserID = x.userID,
                    Content = x.Content
                })
                .ToListAsync();
        }
        public async Task<UserGoalDTO> GetUserGoalByIdAsync(int userGoalId)
        {
            var goal = await _context.UserGoals.FirstOrDefaultAsync(x => x.UserGoalID == userGoalId);

            if (goal == null)
                throw new Exception("UserGoal not found");

            return goal.ToUserGoalDto();
        }
        public async Task<UserGoalDTO> CreateUserGoalAsync(CreateUserGoalRequestDTO requestDto)
        {
            try
            {
                string userId = requestDto.UserID ?? "UnknownUser";

                Console.WriteLine($"[CreateUserGoal] Creating new goal for user {userId}: {requestDto.Content}");

                var entity = requestDto.ToUserGoalFromCreate(userId);
                entity.userID = userId;

                _context.UserGoals.Add(entity);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[CreateUserGoal] ✅ Created new goal with ID {entity.UserGoalID}");

                return entity.ToUserGoalDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateUserGoal] ❌ Error: {ex.Message}");
                Console.WriteLine($"[CreateUserGoal] ❌ Stack trace: {ex.StackTrace}");
                throw new Exception($"Không thể tạo mục tiêu: {ex.Message}");
            }
        }
        public async Task<UserGoalDTO> UpdateUserGoalAsync(int userGoalId, UpdateUserGoalDTO requestDto)
        {
            try
            {
                var entity = await _context.UserGoals.FirstOrDefaultAsync(x => x.UserGoalID == userGoalId);

                if (entity == null)
                    throw new Exception("UserGoal not found");

                Console.WriteLine($"[UpdateUserGoal] Updating goal {userGoalId} for user {entity.userID}");
                Console.WriteLine($"[UpdateUserGoal] Old content: {entity.Content}");
                Console.WriteLine($"[UpdateUserGoal] New content: {requestDto.Content}");

                entity.Content = requestDto.Content;
                entity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                Console.WriteLine($"[UpdateUserGoal] ✅ Goal updated successfully");

                return entity.ToUserGoalDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateUserGoal] ❌ Error: {ex.Message}");
                Console.WriteLine($"[UpdateUserGoal] ❌ Stack trace: {ex.StackTrace}");
                throw new Exception($"Không thể cập nhật mục tiêu: {ex.Message}");
            }
        }
        public async Task DeleteUserGoalAsync(int userGoalId)
        {
            var entity = await _context.UserGoals.FirstOrDefaultAsync(x => x.UserGoalID == userGoalId);

            if (entity == null)
                throw new Exception("UserGoal not found");

            _context.UserGoals.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
