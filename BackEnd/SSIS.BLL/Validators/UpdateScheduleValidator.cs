using FluentValidation;
using SSIS.BLL.DTOs.Schedules;

namespace SSIS.BLL.Validators
{
    public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleDto>
    {
        public UpdateScheduleValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid day of week");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required");

            RuleFor(x => x)
                .Must(dto => dto.EndTime > dto.StartTime)
                .WithMessage("End time must be after start time");

            RuleFor(x => x.Room)
                .NotEmpty().WithMessage("Room is required")
                .MaximumLength(50).WithMessage("Room must be at most 50 characters");
        }
    }
}