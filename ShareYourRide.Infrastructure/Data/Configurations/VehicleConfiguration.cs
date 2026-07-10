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
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasIndex(v => v.PlateNumber).IsUnique();

            builder.Property(v => v.Brand).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Model).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Color).IsRequired().HasMaxLength(30);
            builder.Property(v => v.PlateNumber).IsRequired().HasMaxLength(15);

            builder.HasMany(v => v.Images)
                .WithOne(i => i.Vehicle)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
