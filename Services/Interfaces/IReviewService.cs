using ECM_BE.Models.DTOs.Review;

namespace ECM_BE.Services.Interfaces
{
    public interface IReviewService
    {
            
        Task<List<AllReviewDTO>> GetAllReviewAsync();
        Task<List<ReviewDTO>> GetReviewByCourseIDAsync(int CourseID);
        Task<List<ReviewDTO>> GetReviewByuserIDAsync(string userID);
        Task<ReviewDTO> CreateReviewAsync(string userID, CreateReviewRequestDTO requestDto);
        Task<ReviewDTO> UpdateReviewAsync(int CourseID, UpdateReviewDTO requestDto, string userID);
        Task<bool> DeleteReviewAsync(int CourseID, string currentuserID, bool isAdmin = false);
    }
}

