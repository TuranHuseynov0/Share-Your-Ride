using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
    {
        public void Configure(EntityTypeBuilder<VehicleBrand> builder)
        {
            builder.ToTable("VehicleBrands");
            builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(b => b.Name).IsUnique();

            builder.HasMany(b => b.Models)
                .WithOne(m => m.VehicleBrand)
                .HasForeignKey(m => m.VehicleBrandId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}