using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Application.DTOs.Faq;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IFaqService
    {
        Task<IReadOnlyList<FaqItemDto>> GetAllAsync();
    }
}