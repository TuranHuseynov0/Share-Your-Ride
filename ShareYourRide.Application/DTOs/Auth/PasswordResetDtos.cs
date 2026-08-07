using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required] public string Contact { get; set; } = default!;   // email və ya telefon
        [Required] public OtpChannel Channel { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required] public string Contact { get; set; } = default!;
        [Required] public OtpChannel Channel { get; set; }
        [Required] public string Code { get; set; } = default!;
        [Required, MinLength(6)] public string NewPassword { get; set; } = default!;
        [Required, Compare(nameof(NewPassword))] public string ConfirmNewPassword { get; set; } = default!;
    }
}
