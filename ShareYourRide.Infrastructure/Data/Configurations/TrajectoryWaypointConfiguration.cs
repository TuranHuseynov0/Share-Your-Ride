using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class TrajectoryWaypointConfiguration : IEntityTypeConfiguration<TrajectoryWaypoint>
    {
        public void Configure(EntityTypeBuilder<TrajectoryWaypoint> builder)
        {
            builder.ToTable("TrajectoryWaypoints");

            builder.HasOne(w => w.Trajectory)
                .WithMany(t => t.Waypoints)
                .HasForeignKey(w => w.TrajectoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Stop)
                .WithMany()
                .HasForeignKey(w => w.StopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(w => new { w.TrajectoryId, w.Order }).IsUnique();
        }
    }
}