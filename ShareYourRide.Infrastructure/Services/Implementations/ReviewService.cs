using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Application.DTOs.Review;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Domain.Enums;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewDto> SubmitReviewAsync(Guid reviewerUserId, CreateReviewDto dto)
        {
            var application = await _unitOfWork.RideApplications.GetByIdAsync(dto.RideApplicationId)
                ?? throw new InvalidOperationException("Müraciət tapılmadı.");

            if (application.Status != RideApplicationStatus.Completed)
                throw new InvalidOperationException("Yalnız tamamlanmış gedişlər dəyərləndirilə bilər.");

            var driverTrajectory = await _unitOfWork.Trajectories.GetByIdAsync(application.DriverTrajectoryId)
                ?? throw new InvalidOperationException("Marşrut tapılmadı.");

            Guid revieweeUserId;
            if (reviewerUserId == application.PassengerUserId)
                revieweeUserId = driverTrajectory.UserId;
            else if (reviewerUserId == driverTrajectory.UserId)
                revieweeUserId = application.PassengerUserId;
            else
                throw new InvalidOperationException("Bu gedişə dəyərləndirmə vermək icazəniz yoxdur.");

            var alreadyReviewed = (await _unitOfWork.Reviews.FindAsync(r =>
                r.RideApplicationId == dto.RideApplicationId && r.ReviewerUserId == reviewerUserId)).Any();

            if (alreadyReviewed)
                throw new InvalidOperationException("Bu gedişi artıq dəyərləndirmisiniz.");

            var review = new Review
            {
                RideApplicationId = dto.RideApplicationId,
                ReviewerUserId = reviewerUserId,
                RevieweeUserId = revieweeUserId,
                Stars = dto.Stars
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            await RecalculateRatingAsync(revieweeUserId);

            var reviewer = await _unitOfWork.Users.GetByIdAsync(reviewerUserId);
            var reviewee = await _unitOfWork.Users.GetByIdAsync(revieweeUserId);

            return new ReviewDto
            {
                Id = review.Id,
                RideApplicationId = review.RideApplicationId,
                ReviewerFullName = reviewer != null ? $"{reviewer.FirstName} {reviewer.LastName}" : "N/A",
                RevieweeFullName = reviewee != null ? $"{reviewee.FirstName} {reviewee.LastName}" : "N/A",
                Stars = review.Stars,
                CreatedAt = review.CreatedAt
            };
        }

        private async Task RecalculateRatingAsync(Guid userId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.RevieweeUserId == userId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return;

            user.Rating = reviews.Any() ? Math.Round((decimal)reviews.Average(r => r.Stars), 2) : 0;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
