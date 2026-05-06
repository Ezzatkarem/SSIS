using FluentValidation;
using SSIS.BLL.DTOs.Schedules;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Validators
{
    public class CreateScheduleValidator : AbstractValidator<CreateScheduleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required")
                .MustAsync(CourseExists).WithMessage("Course does not exist");

            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid day of week")
                .Must(d => (int)d >= 1 && (int)d <= 5).WithMessage("Day of week must be between 1 (Saturday) and 5 (Wednesday)");

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

        private async Task<bool> CourseExists(Guid courseId, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            return course != null;
        }
    }
}