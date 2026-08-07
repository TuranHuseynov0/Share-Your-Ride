using System;
using System.ComponentModel.DataAnnotations;

namespace ShareYourRide.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required] public Guid RideApplicationId { get; set; }
        [Required, Range(1, 5)] public int Stars { get; set; }
    }

    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid RideApplicationId { get; set; }
        public string ReviewerFullName { get; set; } = default!;
        public string RevieweeFullName { get; set; } = default!;
        public int Stars { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}