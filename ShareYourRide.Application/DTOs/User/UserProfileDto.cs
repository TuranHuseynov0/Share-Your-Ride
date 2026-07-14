using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.DTOs.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string MaskedFinCode { get; set; } = default!;
        public string? ProfileImagePath { get; set; }
        public Domain.Enums.UserStatus Status { get; set; }
        public bool HasVehicle { get; set; }
    }
}
