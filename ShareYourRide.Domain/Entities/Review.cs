using ShareYourRide.Domain.Common;
using System;

namespace ShareYourRide.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid RideApplicationId { get; set; }
        public RideApplication RideApplication { get; set; } = default!;

        public Guid ReviewerUserId { get; set; }
        public User ReviewerUser { get; set; } = default!;

        public Guid RevieweeUserId { get; set; }
        public User RevieweeUser { get; set; } = default!;

        public int Stars { get; set; } // 1-5
    }
}