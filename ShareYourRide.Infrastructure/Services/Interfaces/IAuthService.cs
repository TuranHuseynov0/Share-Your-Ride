using ShareYourRide.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterPersonalInfoResponseDto> RegisterPersonalInfoAsync(RegisterPersonalInfoDto dto);
        Task RegisterVehicleInfoAsync(RegisterVehicleDto dto);
        Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto);
        Task ResendRegistrationOtpAsync(ResendRegistrationOtpDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
