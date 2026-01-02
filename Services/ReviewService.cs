using ECM_BE.Models.DTOs.Review;
using ECM_BE.Models.Entities;
using ECM_BE.Exceptions;
using ECM_BE.Models.Mapper;
using ECM_BE.Exceptions.Custom;
using Microsoft.EntityFrameworkCore;
using ECM_BE.Services.Interfaces;
using ECM_BE.Data;

namespace ECM_BE.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AllReviewDTO>> GetAllReviewAsync()
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(p => p.User)
                .Select(p => new AllReviewDTO
                {
                    CourseID = p.CourseID,
                    userID = p.userID,
                    UserName = p.User.UserName,
                    FullName = p.User.FullName,
                    Avatar = p.User.Avatar,
                    ReviewScore = p.ReviewScore,
                    ReviewContent = p.ReviewContent,
                    CreatedAt = p.CreatedAt,
                })
                .ToListAsync();
        }

        public async Task<List<ReviewDTO>> GetReviewByCourseIDAsync(int courseID)
        {
            return await _context.Reviews
            .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.CourseID == courseID)
                .Select(p => new ReviewDTO
                {
                    CourseID = p.CourseID,
                    userID = p.userID,
                    UserName = p.User.UserName,
                    FullName = p.User.FullName,
                    Avatar = p.User.Avatar,
                    ReviewScore = p.ReviewScore,
                    ReviewContent = p.ReviewContent,
                    CreatedAt = p.CreatedAt,
                })
                .ToListAsync();
        }

        public async Task<List<ReviewDTO>> GetReviewByuserIDAsync(string userID)
        {
            return await _context.Reviews
            .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.userID == userID)
                .Select(p => new ReviewDTO
                {
                    userID = p.userID,
                    UserName = p.User.UserName,
                    FullName = p.User.FullName,
                    Avatar = p.User.Avatar,
                    CourseID = p.CourseID,
                    ReviewScore = p.ReviewScore,
                    ReviewContent = p.ReviewContent,
                    CreatedAt = p.CreatedAt,
                })
                .ToListAsync();
        }

        public async Task<ReviewDTO> CreateReviewAsync(string userID, CreateReviewRequestDTO requestDto)
        {
            try
            {
                // Check if user has completed the course
                var history = await _context.Histories
                    .FirstOrDefaultAsync(h => h.userID == userID && h.CourseID == requestDto.CourseID);
                
                if (history == null || history.Progress < 100)
                {
                    throw new InvalidOperationException("Bạn cần hoàn thành khóa học trước khi đánh giá.");
                }

                // Check if user already has a review for this course
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.userID == userID && r.CourseID == requestDto.CourseID);
                
                if (existingReview != null)
                {
                    throw new InvalidOperationException("Bạn đã đánh giá khóa học này rồi. Vui lòng chỉnh sửa đánh giá hiện tại.");
                }

                var review = requestDto.ToReviewFromCreate(userID);
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();
                
                // Load the user data
                await _context.Entry(review).Reference(r => r.User).LoadAsync();
                
                return review.ToReviewDto();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ReviewDTO> UpdateReviewAsync(int courseID, UpdateReviewDTO requestDto, string userID)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.CourseID == courseID && r.userID == userID);
            if (review == null)
            {
                throw new NotFoundException("You can only change your own review.");
            }
            review.ReviewScore = requestDto.ReviewScore;
            review.ReviewContent = requestDto.ReviewContent;
            review.CreatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return review.ToReviewDto();
        }

        public async Task<bool> DeleteReviewAsync(int courseID, string currentuserID, bool isAdmin = false)
        {
            // Nếu là admin thì bỏ qua kiểm tra userID, còn không thì chỉ cho xóa đánh giá của chính người dùng.
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.CourseID == courseID
                                              && (isAdmin || r.userID == currentuserID));
            if (review == null)
            {
                throw new NotFoundException("Không tìm thấy đánh giá hoặc bạn không có quyền xóa đánh giá này.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
