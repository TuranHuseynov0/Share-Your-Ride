using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Auth
{
    public class SendOtpDto
    {
        [Required] public Guid UserId { get; set; }
        [Required] public OtpChannel Channel { get; set; }
    }

    public class VerifyOtpDto
    {
        [Required] public Guid UserId { get; set; }
        [Required] public OtpChannel Channel { get; set; }
        [Required] public string Code { get; set; } = default!;
    }
}
