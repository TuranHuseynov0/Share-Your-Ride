using ShareYourRide.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.Auth
{
    public class RegisterPersonalInfoDto
    {
        [Required] public TrajectoryRole Role { get; set; }

        [Required] public string FirstName { get; set; } = default!;
        [Required] public string LastName { get; set; } = default!;
        [Required] public string FinCode { get; set; } = default!;
        [Required] public DateTime BirthDate { get; set; }

        [Required, EmailAddress] public string Email { get; set; } = default!;
        [Required, Phone] public string PhoneNumber { get; set; } = default!;

        [Required, MinLength(8)] public string Password { get; set; } = default!;
        [Required, Compare(nameof(Password))] public string ConfirmPassword { get; set; } = default!;
    }

    public class RegisterPersonalInfoResponseDto
    {
        public Guid UserId { get; set; }
        public string MaskedPhoneNumber { get; set; } = default!;
        public int OtpExpirySeconds { get; set; }
        public bool RequiresVehicleInfo { get; set; }   // Role == Driver isə true
    }
}
