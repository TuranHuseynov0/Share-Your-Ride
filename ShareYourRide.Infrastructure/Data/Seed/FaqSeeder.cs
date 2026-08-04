using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Seed
{
    public static class FaqSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.FaqItems.AnyAsync()) return;

            var items = new[]
            {
                new FaqItem { Question = "Qeydiyyat necə işləyir?", Answer = "Şəxsi məlumatları daxil edib email OTP kodu ilə hesabınızı təsdiqləyirsiniz.", Order = 1 },
                new FaqItem { Question = "Sürücü kimi nə vaxt marşrut yarada bilərəm?", Answer = "Admin sənədlərinizi təsdiqlədikdən sonra marşrut yarada bilərsiniz.", Order = 2 },
                new FaqItem { Question = "Balansımı necə artıra bilərəm?", Answer = "Balans bölməsindən minimum 5 AZN məbləğində ödəniş edə bilərsiniz.", Order = 3 }
            };

            context.FaqItems.AddRange(items);
            await context.SaveChangesAsync();
        }
    }
}