using Microsoft.EntityFrameworkCore;
using ShareYourRide.Domain.Entities;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Data.Seed
{
    public static class StopSeeder
    {
        // Sıra Dərnəgüldən başlayaraq xətt üzrə ardıcıllığı əks etdirir.
        // Order sahəsi məhz bu massivin indeksinə əsaslanır.
        public static readonly string[] StopNames = new string[]
        {
            "Dərnəgül",
            "Azadlıq",
            "Nəsimi",
            "Əcəmi",
            "20 Yanvar",
            "İnşaatçılar",
            "Elmlər",
            "Nizami",
            "28 May",
            "Gənclik",
            "Nərimanov",
            "Ulduz",
            "Qara Qarayev",
            "Neftçilər",
            "Xalqlar Dostluğu",
            "Əhmədli",
            "Həzi Aslanov"
        };

        public static async Task SeedStopAsync(AppDbContext context)
        {
            for (int i = 0; i < StopNames.Length; i++)
            {
                var name = StopNames[i];
                var existing = await context.Stops.FirstOrDefaultAsync(s => s.Name == name);

                if (existing == null)
                {
                    context.Stops.Add(new Stop { Name = name, Order = i, IsActive = true });
                }
                else if (existing.Order != i)
                {
                    // Sıra dəyişibsə (məs. "Elmlər" araya əlavə olunub), mövcud qeydi yeniləyirik
                    existing.Order = i;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}