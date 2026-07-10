using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            builder.HasMany(t => t.Applications)
                .WithOne(a => a.DriverTrajectory)
                .HasForeignKey(a => a.DriverTrajectoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => new { t.UserId, t.Day, t.Role });
        }
    }
}
