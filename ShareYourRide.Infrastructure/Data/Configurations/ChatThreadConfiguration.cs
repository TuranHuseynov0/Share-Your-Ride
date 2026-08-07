using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Configurations
{
    public class ChatThreadConfiguration : IEntityTypeConfiguration<ChatThread>
    {
        public void Configure(EntityTypeBuilder<ChatThread> builder)
        {
            builder.ToTable("ChatThreads");

            builder.HasOne(c => c.RideApplication)
                .WithMany()
                .HasForeignKey(c => c.RideApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.DriverUser)
                .WithMany()
                .HasForeignKey(c => c.DriverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.PassengerUser)
                .WithMany()
                .HasForeignKey(c => c.PassengerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.RideApplicationId).IsUnique();

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.ChatThread)
                .HasForeignKey(m => m.ChatThreadId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}