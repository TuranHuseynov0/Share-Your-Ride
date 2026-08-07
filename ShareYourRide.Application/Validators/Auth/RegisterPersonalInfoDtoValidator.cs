using FluentValidation;
using ShareYourRide.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.Validators.Auth
{
    public class RegisterPersonalInfoDtoValidator : AbstractValidator<RegisterPersonalInfoDto>
    {
        public RegisterPersonalInfoDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.FinCode)
                .NotEmpty()
                .Length(7).WithMessage("FIN kod 7 simvoldan ibarət olmalıdır!")
                .Matches("^[A-Za-z0-9]+$").WithMessage("FIN kod yalnız hərf və rəqəmlərdən ibarət ola bilər.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.UtcNow.AddYears(-18)).WithMessage("İstifadəçi 18 yaşından yuxarı olmalıdır.");

            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+994(50|51|55|70|77|99|10|60)")
                .WithMessage("Telefon nömrəsi düzgün formatda deyil! (məs: +994501234567).");

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Şifrədə ən azı 1 böyük hərf olmalıdır.")
                .Matches("[0-9]").WithMessage("Şifrədə ən azı 1 rəqəm olmalıdır.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Şifrələr uyğun gəlmir.");
        }
    }
}
