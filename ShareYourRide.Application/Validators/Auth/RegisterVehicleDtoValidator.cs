using FluentValidation;
using ShareYourRide.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.Validators.Auth
{
    public class RegisterVehicleDtoValidator : AbstractValidator<RegisterVehicleDto>
    {
        public RegisterVehicleDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Brand).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Color).NotEmpty().MaximumLength(50);

            RuleFor(x => x.Year)
                .NotEmpty()
                .GreaterThanOrEqualTo(1990)
                .LessThanOrEqualTo(DateTime.UtcNow.Year + 1);

            RuleFor(x => x.PlateNumber)
                .NotEmpty()
                .Matches(@"^\d{2}-[A-Z]{2}-\d{3}$")
                .WithMessage("Qeydiyyat nişanı formatı düzgün deyil(məs: 10 - AA - 123).");

            RuleFor(x => x.FrontImagePath).NotEmpty();
            RuleFor(x => x.BackImagePath).NotEmpty();
            RuleFor(x => x.LeftImagePath).NotEmpty();
            RuleFor(x => x.RightImagePath).NotEmpty();
        }
    }
}
