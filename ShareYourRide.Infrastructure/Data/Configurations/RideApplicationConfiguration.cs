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
    public class RideApplicationConfiguration : IEntityTypeConfiguration<RideApplication>
    {
        public void Configure(EntityTypeBuilder<RideApplication> builder)
        {
            builder.ToTable("RideApplications");

            builder.HasOne(a => a.PassengerUser)
                .WithMany()
                .HasForeignKey(a => a.PassengerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.DriverTrajectoryId, a.PassengerUserId }).IsUnique();
        }
    }
}
