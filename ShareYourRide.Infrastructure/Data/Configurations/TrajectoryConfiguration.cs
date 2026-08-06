using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class TrajectoryConfiguration : IEntityTypeConfiguration<Trajectory>
    {
        public void Configure(EntityTypeBuilder<Trajectory> builder)
        {
            builder.ToTable("Trajectories");

            builder.HasOne(t => t.StartStop)
                .WithMany()
                .HasForeignKey(t => t.StartStopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.EndStop)
                .WithMany()
                .HasForeignKey(t => t.EndStopId)
                .OnDelete(DeleteBehavior.Restrict);

            // Applications əlaqəsi artıq RideApplicationConfiguration.cs-də konfiqurasiya olunur — burada təkrarlanmır

            builder.HasIndex(t => new { t.UserId, t.Day, t.Role });
            builder.HasIndex(t => t.ScheduleGroupId);
        }
    }
}