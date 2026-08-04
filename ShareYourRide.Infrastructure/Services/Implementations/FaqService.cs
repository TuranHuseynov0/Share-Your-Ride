using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Application.DTOs.Faq;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class FaqService : IFaqService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FaqService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<FaqItemDto>> GetAllAsync()
        {
            var items = await _unitOfWork.FaqItems.FindAsync(f => f.IsActive);
            return items.OrderBy(f => f.Order)
                .Select(f => new FaqItemDto { Id = f.Id, Question = f.Question, Answer = f.Answer })
                .ToList();
        }
    }
}