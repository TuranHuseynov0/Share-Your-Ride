using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class RideApplicationConfiguration : IEntityTypeConfiguration<RideApplication>
    {
        public void Configure(EntityTypeBuilder<RideApplication> builder)
        {
            builder.ToTable("RideApplications");

            // Sürücünün trayektoriyası silinərsə, ona bağlı bütün müraciətlər də silinsin (bu, TEK icazəli cascade yoludur)
            builder.HasOne(a => a.DriverTrajectory)
                .WithMany(t => t.Applications)
                .HasForeignKey(a => a.DriverTrajectoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sərnişinin User-i silinərsə, cascade YOX — əl ilə idarə olunmalıdır
            builder.HasOne(a => a.PassengerUser)
                .WithMany()
                .HasForeignKey(a => a.PassengerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sərnişinin trayektoriyası silinərsə, cascade YOX — bu, ikinci cascade yolunun qarşısını alır
            builder.HasOne(a => a.PassengerTrajectory)
                .WithMany()
                .HasForeignKey(a => a.PassengerTrajectoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.Price).HasColumnType("decimal(18,2)");

            builder.HasIndex(a => new { a.DriverTrajectoryId, a.PassengerUserId }).IsUnique();
        }
    }
}