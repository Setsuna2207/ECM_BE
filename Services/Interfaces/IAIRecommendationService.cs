using ECM_BE.Models.DTOs.AIRecommendation;

namespace ECM_BE.Services.Interfaces
{
    public interface IAIRecommendationService
    {
        Task<GoalAnalysisResponseDTO> AnalyzeGoalAndRecommendTestAsync(GoalAnalysisRequestDTO request);
        Task<CourseRecommendationResponseDTO> RecommendCoursesBasedOnResultAsync(CourseRecommendationRequestDTO request);
        Task<GoalAnalysisResponseDTO> GenerateFinalAssessmentAsync(int learningPathId);
        Task<bool> UpdateCourseCompletionAsync(int learningPathId, int courseId);
    }
}
