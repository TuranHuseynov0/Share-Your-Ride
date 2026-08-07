using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Application.DTOs.VehicleCatalog;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IVehicleCatalogService
    {
        Task<IReadOnlyList<VehicleBrandDto>> GetBrandsAsync();
        Task<IReadOnlyList<VehicleModelDto>> GetModelsByBrandAsync(Guid brandId);
        Task<IReadOnlyList<VehicleColorDto>> GetColorsAsync();
    }
}
