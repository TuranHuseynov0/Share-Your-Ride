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
        public string? Bio { get; set; }
        public Domain.Enums.UserStatus Status { get; set; }
        public bool HasVehicle { get; set; }
        public string FullName => $"{FirstName}".Trim();

        // YENİ
        public decimal Rating { get; set; }
        public int MemberSinceYear { get; set; }
        public int CompletedRideCount { get; set; }
        public decimal CancellationRatePercent { get; set; }
    }
}