using Microsoft.EntityFrameworkCore;
using ShareYourRide.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Data.Seed
{
    public static class StopSeeder
    {
        public static readonly string[] StopNames = new string[]
        {
            "Dərnəgül", "Azadlıq", "Nəsimi", "Əcəmi", "20 Yanvar", "İnşaatçılar", "Nizami", "28 May", "Gənclik", "Nərimanov",
            "Ulduz", "Qara Qarayev", "Neftçilər", "Əhmədli", "Həzi Aslanov",
            "Binə", "Biləcəri", "Binəqədi"
        };

        public static async Task SeedStopAsync(AppDbContext context)
        {
            foreach(var name in StopNames)
            {
                var exists = await context.Stops.AnyAsync(s => s.Name == name);
                if (!exists)
                    context.Stops.Add(new Stop { Name = name });
            }
            await context.SaveChangesAsync();
        }
    }
}
