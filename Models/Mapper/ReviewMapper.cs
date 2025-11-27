using ECM_BE.Models.Entities;
using ECM_BE.Models.DTOs.Review;

namespace ECM_BE.Models.Mapper
{
    public static class ReviewMapper
    {
        public static Review ToReviewFromCreate(this CreateReviewRequestDTO requestDto, string userID)
        {
            return new Review
            {
                CourseID = requestDto.CourseID,
                userID = userID,
                ReviewScore = requestDto.ReviewScore,
                ReviewContent = requestDto.ReviewContent,
                CreatedAt = requestDto.CreatedAt,
            };
        }
        public static ReviewDTO ToReviewDto(this Review requestDto)
        {
            return new ReviewDTO
            {
                CourseID = requestDto.CourseID,
                userID = requestDto.userID,
                ReviewScore = requestDto.ReviewScore,
                ReviewContent = requestDto.ReviewContent,
                CreatedAt = requestDto.CreatedAt,
            };
        }
    }
}
