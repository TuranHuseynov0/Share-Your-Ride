using ShareYourRide.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class PendingRegistration
    {
        public RegisterPersonalInfoDto PersonalInfo { get; set; } = default!;
        public RegisterVehicleDto? VehicleInfo { get; set; }
        public string OtpCode { get; set; } = default!;
        public int FailedAttempts { get; set; }
    }
}
