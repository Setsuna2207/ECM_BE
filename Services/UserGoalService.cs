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
            string userId = requestDto.UserID ?? "UnknownUser";

            var entity = requestDto.ToUserGoalFromCreate(userId);
            entity.userID = userId;

            _context.UserGoals.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToUserGoalDto();
        }
        public async Task<UserGoalDTO> UpdateUserGoalAsync(int userGoalId, UpdateUserGoalDTO requestDto)
        {
            var entity = await _context.UserGoals.FirstOrDefaultAsync(x => x.UserGoalID == userGoalId);

            if (entity == null)
                throw new Exception("UserGoal not found");

            entity.Content = requestDto.Content;

            await _context.SaveChangesAsync();

            return entity.ToUserGoalDto();
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
