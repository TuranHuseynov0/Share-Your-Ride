using ShareYourRide.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Domain.Entities
{
    public class Stop : BaseEntity
    {
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}
