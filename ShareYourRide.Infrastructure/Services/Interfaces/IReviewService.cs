using ShareYourRide.Application.DTOs.Review;
using System;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> SubmitReviewAsync(Guid reviewerUserId, CreateReviewDto dto);
    }
}