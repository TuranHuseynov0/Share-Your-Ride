using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Template
{
    public class TemplateDto
    {
        [Required] public Guid Id { get; set; }
        public string StartStopName { get; set; } = default!;
        public string EndStopName { get; set; } = default!;
    }
}
