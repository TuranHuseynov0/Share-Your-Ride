using ShareYourRide.Application.DTOs.Stop;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class StopService : IStopService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StopService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<StopDto>> GetAllAsync()
        {
            var stops = await _unitOfWork.Stops.FindAsync(s => s.IsActive);
            return stops.Select(s => new StopDto { Id = s.Id, Name = s.Name}).ToList();
        }
    }
}
