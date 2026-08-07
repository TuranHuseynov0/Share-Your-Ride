using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Auth
{
    public class VerifyRegistrationOtpDto
    {
        [Required] public Guid UserId { get; set; }
        [Required] public string Code { get; set; } = default!;
    }

    public class ResendRegistrationOtpDto
    {
        [Required] public Guid UserId { get; set; }
    }
}
