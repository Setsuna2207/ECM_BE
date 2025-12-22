using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.PlacementTest;

namespace ECM_BE.Models.Mapper
{
    public static class PlacementTestMapper
    {
        public static PlacementTest ToPlacementTestFromCreate(this CreatePlacementTestRequestDTO requestDto)
        {
            return new PlacementTest
            {
                Title = requestDto.Title,
                Description = requestDto.Description,
                Duration = requestDto.Duration,
                TotalQuestions = requestDto.TotalQuestions,
                QuestionFileURL = requestDto.QuestionFileURL,
                MediaURL = requestDto.MediaURL,
            };
        }
        public static PlacementTestDTO ToPlacementTestDto(this PlacementTest requestDto)
        {
            return new PlacementTestDTO
            {
                Title = requestDto.Title,
                Description = requestDto.Description,
                Duration = requestDto.Duration,
                TotalQuestions = requestDto.TotalQuestions,
                QuestionFileURL = requestDto.QuestionFileURL,
                MediaURL = requestDto.MediaURL,
            };
        }
    }
}
