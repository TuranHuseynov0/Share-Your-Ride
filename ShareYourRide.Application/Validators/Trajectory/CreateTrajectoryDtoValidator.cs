using FluentValidation;
using ShareYourRide.Application.DTOs.Trajectory;
using ShareYourRide.Domain.Enums;

namespace ShareYourRide.Application.Validators.Trajectory
{
    public class CreateTrajectoryDtoValidator : AbstractValidator<CreateTrajectoryDto>
    {
        public CreateTrajectoryDtoValidator()
        {
            RuleFor(x => x.DaysOfWeek)
                .NotEmpty().WithMessage("Ən azı bir gün seçilməlidir.");

            RuleFor(x => x.StartStopId).NotEmpty();
            RuleFor(x => x.EndStopId).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.StartStopId != x.EndStopId)
                .WithMessage("Başlanğıc və son dayanacaq eyni ola bilməz.");

            When(x => x.Role == TrajectoryRole.Driver, () =>
            {
                RuleFor(x => x.SeatCount)
                    .NotNull().WithMessage("Sürücü üçün oturacaq sayı seçilməlidir.")
                    .GreaterThan(0).WithMessage("Oturacaq sayı 0-dan böyük olmalıdır.")
                    .LessThanOrEqualTo(8).WithMessage("Oturacaq sayı 8-dən çox ola bilməz.");
            });
        }
    }
}