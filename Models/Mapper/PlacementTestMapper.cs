using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.PlacementTest;
using Newtonsoft.Json;

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
                Sections = requestDto.Sections != null
                    ? JsonConvert.SerializeObject(requestDto.Sections)
                    : null
            };
        }

        public static PlacementTestDTO ToPlacementTestDto(this PlacementTest test)
        {
            return new PlacementTestDTO
            {
                TestID = test.TestID,
                Title = test.Title,
                Description = test.Description,
                Duration = test.Duration,
                TotalQuestions = test.TotalQuestions,
                QuestionFileURL = test.QuestionFileURL,
                MediaURL = test.MediaURL,
                Sections = !string.IsNullOrEmpty(test.Sections)
                    ? JsonConvert.DeserializeObject<List<TestSectionDTO>>(test.Sections)
                    : null
            };
        }
    }
}