using ECM_BE.Data;
using ECM_BE.Models.DTOs.PlacementTest;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ECM_BE.Services
{
    public class PlacementTestService : IPlacementTestService
    {
        private readonly AppDbContext _context;

        public PlacementTestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AllPlacementTestDTO>> GetAllPlacementTestsAsync()
        {
            return await _context.PlacementTests
                .AsNoTracking()
                .Select(pt => new AllPlacementTestDTO
                {
                    TestID = pt.TestID,
                    Title = pt.Title,
                    Description = pt.Description,
                    TotalQuestions = pt.TotalQuestions,
                    Duration = pt.Duration,
                    QuestionFileURL = pt.QuestionFileURL,
                    MediaURL = pt.MediaURL,
                    Category = pt.Category,
                    Level = pt.Level
                })
                .ToListAsync();
        }

        public async Task<PlacementTestDTO> GetPlacementTestByIdAsync(int TestId)
        {
            var pt = await _context.PlacementTests
                .AsNoTracking()
                .FirstOrDefaultAsync(pt => pt.TestID == TestId);

            if (pt == null)
                throw new Exception("Không tìm thấy test");

            return pt.ToPlacementTestDto();
        }

        public async Task<PlacementTestDTO> CreatePlacementTestAsync(CreatePlacementTestRequestDTO requestDto)
        {
            var pt = requestDto.ToPlacementTestFromCreate();

            _context.PlacementTests.Add(pt);
            await _context.SaveChangesAsync();

            return pt.ToPlacementTestDto();
        }

        public async Task<PlacementTestDTO> UpdatePlacementTestAsync(int TestId, UpdatePlacementTestDTO requestDto)
        {
            var pt = await _context.PlacementTests
                .FirstOrDefaultAsync(pt => pt.TestID == TestId);

            if (pt == null)
                throw new Exception("Placement test not found");

            pt.Title = requestDto.Title;
            pt.Description = requestDto.Description;
            pt.TotalQuestions = requestDto.TotalQuestions;
            pt.Duration = requestDto.Duration;
            pt.QuestionFileURL = requestDto.QuestionFileURL;
            pt.MediaURL = requestDto.MediaURL;
            pt.Category = requestDto.Category ?? pt.Category;
            pt.Level = requestDto.Level ?? pt.Level;
            pt.Sections = requestDto.Sections != null
                ? JsonConvert.SerializeObject(requestDto.Sections)
                : pt.Sections;

            await _context.SaveChangesAsync();

            return pt.ToPlacementTestDto();
        }

        public async Task DeletePlacementTestAsync(int TestId)
        {
            var placementTest = await _context.PlacementTests
                .FirstOrDefaultAsync(pt => pt.TestID == TestId);

            if (placementTest == null)
                throw new Exception("Placement test not found");

            _context.PlacementTests.Remove(placementTest);
            await _context.SaveChangesAsync();
        }
    }
}