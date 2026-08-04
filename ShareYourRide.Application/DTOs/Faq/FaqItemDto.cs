using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ShareYourRide.Application.DTOs.Faq
{
    public class FaqItemDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
    }
}