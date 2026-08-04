using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShareYourRide.Domain.Entities;

namespace ShareYourRide.Infrastructure.Data.Seed
{
    public static class VehicleCatalogSeeder
    {
        private static readonly Dictionary<string, string[]> BrandModels = new()
        {
            ["Toyota"] = new[] { "Corolla", "Camry", "Prius", "RAV4", "Land Cruiser" },
            ["Hyundai"] = new[] { "Elantra", "Sonata", "Tucson", "Accent" },
            ["Kia"] = new[] { "Rio", "Cerato", "Sportage", "Optima" },
            ["Chevrolet"] = new[] { "Cobalt", "Lacetti", "Malibu", "Nexia" },
            ["Mercedes-Benz"] = new[] { "C-Class", "E-Class", "S-Class", "GLE" },
            ["BMW"] = new[] { "3 Series", "5 Series", "X5" },
            ["Lada (VAZ)"] = new[] { "Granta", "Vesta", "Priora", "Niva" }
        };

        private static readonly (string Name, string Hex)[] Colors =
        {
            ("Ağ", "#FFFFFF"), ("Qara", "#000000"), ("Gümüşü", "#C0C0C0"),
            ("Boz", "#808080"), ("Qırmızı", "#FF0000"), ("Mavi", "#0000FF"),
            ("Yaşıl", "#008000")
        };

        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.VehicleBrands.AnyAsync())
            {
                foreach (var (brandName, models) in BrandModels)
                {
                    var brand = new VehicleBrand { Name = brandName };
                    foreach (var modelName in models)
                        brand.Models.Add(new VehicleModel { Name = modelName });

                    context.VehicleBrands.Add(brand);
                }
            }

            if (!await context.VehicleColors.AnyAsync())
            {
                foreach (var (name, hex) in Colors)
                    context.VehicleColors.Add(new VehicleColor { Name = name, HexCode = hex });
            }

            await context.SaveChangesAsync();
        }
    }
}
