using ShareYourRide.Application.DTOs.Stop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IStopService
    {
        Task<IReadOnlyList<StopDto>> GetAllAsync();
    }
}
