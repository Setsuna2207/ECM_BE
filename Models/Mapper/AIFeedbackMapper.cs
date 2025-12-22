using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.AIFeedback;

namespace ECM_BE.Models.Mapper
{
    public static class AIFeedbackMapper
    {
        public static AIFeedback ToAIFeedbackFromCreate(this CreateAIFeedbackRequestDTO requestDto)
        {
            return new AIFeedback
            {
                ResultID = requestDto.ResultID,
                WeakSkill = requestDto.WeakSkill,
                RcmCourses = requestDto.RcmCourses,
                CreatedAt = requestDto.CreatedAt,
                FeedbackSummary = requestDto.FeedbackSummary,
            };
        }
        public static AIFeedbackDTO ToAIFeedbackDto(this AIFeedback requestDto)
        {
            return new AIFeedbackDTO
            {
                FeedbackID = requestDto.FeedbackID,
                ResultID = requestDto.ResultID,
                WeakSkill = requestDto.WeakSkill,
                RcmCourses = requestDto.RcmCourses,
                CreatedAt = requestDto.CreatedAt,
                FeedbackSummary = requestDto.FeedbackSummary,
            };
        }
    }
}
