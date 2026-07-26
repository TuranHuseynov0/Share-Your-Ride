using FluentValidation;
using ShareYourRide.Application.DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Application.Validators.Wallet
{
    public class TopUpValidator : AbstractValidator<TopUpDto>
    {
        public TopUpValidator()
        {
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(5).WithMessage("Minimum top-up 5 AZN-dir.");
        }
    }
}
