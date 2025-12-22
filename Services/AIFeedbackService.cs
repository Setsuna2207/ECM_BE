using ECM_BE.Data;
using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.AIFeedback;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class AIFeedbackService : IAIFeedbackService
    {
        private readonly AppDbContext _context;

        public AIFeedbackService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<AllAIFeedbackDTO>> GetAllAIFeedbacksAsync()
        {
            return await _context.AIFeedbacks
                .Select(x => new AllAIFeedbackDTO
                {
                    FeedbackID = x.FeedbackID,
                    ResultID = x.ResultID,
                    WeakSkill = x.WeakSkill,
                    RcmCourses = x.RcmCourses,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<AIFeedbackDTO> GetAIFeedbackByIdAsync(int aiFeedbackId)
        {
            var entity = await _context.AIFeedbacks.FirstOrDefaultAsync(x => x.FeedbackID == aiFeedbackId);

            if (entity == null)
                throw new Exception("AI Feedback not found");

            return entity.ToAIFeedbackDto();
        }
        public async Task<AIFeedbackDTO> CreateAIFeedbackAsync(CreateAIFeedbackRequestDTO requestDto)
        {
            var entity = requestDto.ToAIFeedbackFromCreate();

            _context.AIFeedbacks.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToAIFeedbackDto();
        }
        public async Task<AIFeedbackDTO> UpdateAIFeedbackAsync(int aiFeedbackId, UpdateAIFeedbackDTO requestDto)
        {
            var entity = await _context.AIFeedbacks.FirstOrDefaultAsync(x => x.FeedbackID == aiFeedbackId);

            if (entity == null)
                throw new Exception("AI Feedback not found");

            entity.WeakSkill = requestDto.WeakSkill;
            entity.RcmCourses = requestDto.RcmCourses;
            entity.FeedbackSummary = requestDto.FeedbackSummary;

            await _context.SaveChangesAsync();

            return entity.ToAIFeedbackDto();
        }
        public async Task DeleteAIFeedbackAsync(int aiFeedbackId)
        {
            var entity = await _context.AIFeedbacks.FirstOrDefaultAsync(x => x.FeedbackID == aiFeedbackId);

            if (entity == null)
                throw new Exception("AI Feedback not found");

            _context.AIFeedbacks.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
