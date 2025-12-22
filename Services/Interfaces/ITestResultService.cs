using ECM_BE.Models.DTOs.TestResult;

namespace ECM_BE.Services.Interfaces
{
    public interface ITestResultService
    {
        Task<List<AllTestResultDTO>> GetAllTestResultsAsync();
        Task<TestResultDTO> GetTestResultByIdAsync(int testResultId);
        Task<TestResultDTO> CreateTestResultAsync(CreateTestResultRequestDTO requestDto);
        Task<TestResultDTO> UpdateTestResultAsync(int ResultId, UpdateTestResultDTO requestDto);
        Task DeleteTestResultAsync(int ResultId);
    }
}
