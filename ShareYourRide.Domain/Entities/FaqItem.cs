using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareYourRide.Domain.Common;

namespace ShareYourRide.Domain.Entities
{
    public class FaqItem : BaseEntity
    {
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}