using FluentValidation;
using ShareYourRide.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.Validators.Auth
{
    public class ForgotPasswordValidatorDto : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidatorDto()
        {
        }
    }
}
