using ECM_BE.Data;
using ECM_BE.Models.DTOs.LearningPath;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class LearningPathService : ILearningPathService
    {
        private readonly AppDbContext _context;

        public LearningPathService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LearningPathDTO?> GetActiveLearningPathByUserIdAsync(string userId)
        {
            // Debug: Show all learning paths for this user
            var allPaths = await _context.LearningPaths
                .Include(lp => lp.UserGoal)
                .Where(lp => lp.userID == userId)
                .OrderByDescending(lp => lp.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"[GetActiveLearningPath] User {userId} has {allPaths.Count} total learning paths:");
            foreach (var path in allPaths)
            {
                Console.WriteLine($"  - ID: {path.LearningPathID}, Status: {path.Status}, Goal: {path.UserGoal?.Content}, Created: {path.CreatedAt}");
            }

            // Get the most recent non-archived, non-completed learning path
            var learningPath = await _context.LearningPaths
                .Include(lp => lp.UserGoal)
                .Include(lp => lp.InitialTest)
                .Include(lp => lp.FinalTest)
                .Where(lp => lp.userID == userId && lp.Status != "Completed" && lp.Status != "Archived")
                .OrderByDescending(lp => lp.CreatedAt)
                .FirstOrDefaultAsync();

            if (learningPath == null)
            {
                Console.WriteLine($"[GetActiveLearningPath] No active learning path found for user {userId}");
                return null;
            }

            Console.WriteLine($"[GetActiveLearningPath] Selected learning path ID: {learningPath.LearningPathID}");
            Console.WriteLine($"[GetActiveLearningPath] Goal: {learningPath.UserGoal?.Content}");
            Console.WriteLine($"[GetActiveLearningPath] Status: {learningPath.Status}");
            Console.WriteLine($"[GetActiveLearningPath] Created: {learningPath.CreatedAt}");

            return new LearningPathDTO
            {
                LearningPathID = learningPath.LearningPathID,
                UserID = learningPath.userID,
                UserGoalID = learningPath.UserGoalID,
                GoalContent = learningPath.UserGoal?.Content,
                InitialTestID = learningPath.InitialTestID,
                InitialTestTitle = learningPath.InitialTest?.Title,
                InitialResultID = learningPath.InitialResultID,
                RecommendedCourses = learningPath.RecommendedCourses,
                CompletedCourses = learningPath.CompletedCourses,
                FinalTestID = learningPath.FinalTestID,
                FinalTestTitle = learningPath.FinalTest?.Title,
                FinalResultID = learningPath.FinalResultID,
                Status = learningPath.Status,
                CreatedAt = learningPath.CreatedAt,
                UpdatedAt = learningPath.UpdatedAt,
                CompletedAt = learningPath.CompletedAt
            };
        }

        public async Task<LearningPathDTO> GetLearningPathByIdAsync(int learningPathId)
        {
            var learningPath = await _context.LearningPaths
                .Include(lp => lp.UserGoal)
                .Include(lp => lp.InitialTest)
                .Include(lp => lp.FinalTest)
                .FirstOrDefaultAsync(lp => lp.LearningPathID == learningPathId);

            if (learningPath == null)
                throw new Exception("Không tìm thấy lộ trình học tập");

            return new LearningPathDTO
            {
                LearningPathID = learningPath.LearningPathID,
                UserID = learningPath.userID,
                UserGoalID = learningPath.UserGoalID,
                GoalContent = learningPath.UserGoal?.Content,
                InitialTestID = learningPath.InitialTestID,
                InitialTestTitle = learningPath.InitialTest?.Title,
                InitialResultID = learningPath.InitialResultID,
                RecommendedCourses = learningPath.RecommendedCourses,
                CompletedCourses = learningPath.CompletedCourses,
                FinalTestID = learningPath.FinalTestID,
                FinalTestTitle = learningPath.FinalTest?.Title,
                FinalResultID = learningPath.FinalResultID,
                Status = learningPath.Status,
                CreatedAt = learningPath.CreatedAt,
                UpdatedAt = learningPath.UpdatedAt,
                CompletedAt = learningPath.CompletedAt
            };
        }

        public async Task<LearningPathDTO> UpdateLearningPathAsync(int learningPathId, UpdateLearningPathDTO dto)
        {
            var learningPath = await _context.LearningPaths
                .FirstOrDefaultAsync(lp => lp.LearningPathID == learningPathId);

            if (learningPath == null)
                throw new Exception("Không tìm thấy lộ trình học tập");

            if (dto.InitialTestID.HasValue)
                learningPath.InitialTestID = dto.InitialTestID;

            if (dto.InitialResultID.HasValue)
                learningPath.InitialResultID = dto.InitialResultID;

            if (dto.RecommendedCourses != null)
                learningPath.RecommendedCourses = dto.RecommendedCourses;

            if (dto.CompletedCourses != null)
                learningPath.CompletedCourses = dto.CompletedCourses;

            if (dto.FinalTestID.HasValue)
                learningPath.FinalTestID = dto.FinalTestID;

            if (dto.FinalResultID.HasValue)
                learningPath.FinalResultID = dto.FinalResultID;

            if (dto.Status != null)
                learningPath.Status = dto.Status;

            learningPath.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetLearningPathByIdAsync(learningPathId);
        }
    }
}
