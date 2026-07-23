using ShareYourRide.Application.DTOs.Template;
using ShareYourRide.Application.DTOs.Trajectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface ITrajectoryService
    {
        Task<CreateTrajectoryResponseDto> CreateAsync(Guid userId, CreateTrajectoryDto dto);
        Task<CreateTrajectoryResponseDto> CreateFromTemplateAsync(Guid userId, CreateFromTemplateDto dto);
        Task<IReadOnlyList<TemplateDto>> GetMyTemplatesAsync(Guid userId);
    }
}
