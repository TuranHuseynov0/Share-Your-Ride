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
    public class FaqItemConfiguration : IEntityTypeConfiguration<FaqItem>
    {
        public void Configure(EntityTypeBuilder<FaqItem> builder)
        {
            builder.ToTable("FaqItems");
            builder.Property(f => f.Question).IsRequired().HasMaxLength(300);
            builder.Property(f => f.Answer).IsRequired().HasMaxLength(2000);
        }
    }
}