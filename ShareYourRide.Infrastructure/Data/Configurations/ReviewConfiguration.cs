using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasOne(r => r.RideApplication)
                .WithMany()
                .HasForeignKey(r => r.RideApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.ReviewerUser)
                .WithMany()
                .HasForeignKey(r => r.ReviewerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RevieweeUser)
                .WithMany()
                .HasForeignKey(r => r.RevieweeUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.RideApplicationId, r.ReviewerUserId }).IsUnique();
        }
    }
}