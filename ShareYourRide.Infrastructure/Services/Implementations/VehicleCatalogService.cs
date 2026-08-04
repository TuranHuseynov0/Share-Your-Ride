using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Application.DTOs.VehicleCatalog;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class VehicleCatalogService : IVehicleCatalogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleCatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<VehicleBrandDto>> GetBrandsAsync()
        {
            var brands = await _unitOfWork.VehicleBrands.FindAsync(b => b.IsActive);
            return brands.OrderBy(b => b.Name)
                .Select(b => new VehicleBrandDto { Id = b.Id, Name = b.Name })
                .ToList();
        }

        public async Task<IReadOnlyList<VehicleModelDto>> GetModelsByBrandAsync(Guid brandId)
        {
            var models = await _unitOfWork.VehicleModels.FindAsync(m => m.VehicleBrandId == brandId && m.IsActive);
            return models.OrderBy(m => m.Name)
                .Select(m => new VehicleModelDto { Id = m.Id, Name = m.Name })
                .ToList();
        }

        public async Task<IReadOnlyList<VehicleColorDto>> GetColorsAsync()
        {
            var colors = await _unitOfWork.VehicleColors.FindAsync(c => c.IsActive);
            return colors.OrderBy(c => c.Name)
                .Select(c => new VehicleColorDto { Id = c.Id, Name = c.Name, HexCode = c.HexCode })
                .ToList();
        }
    }
}