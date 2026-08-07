using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class VehicleColorConfiguration : IEntityTypeConfiguration<VehicleColor>
    {
        public void Configure(EntityTypeBuilder<VehicleColor> builder)
        {
            builder.ToTable("VehicleColors");
            builder.Property(c => c.Name).IsRequired().HasMaxLength(30);
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}