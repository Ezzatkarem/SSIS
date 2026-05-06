using FluentValidation;
using SSIS.BLL.DTOs.Attendances;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Validators
{
    public class TakeAttendanceValidator : AbstractValidator<TakeAttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TakeAttendanceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required")
                .MustAsync(CourseExists).WithMessage("Course does not exist");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future");

            RuleFor(x => x.attendances)
                .NotEmpty().WithMessage("At least one attendance record is required");

            RuleForEach(x => x.attendances)
                .SetValidator(new StudentAttendanceValidator());
        }

        private async Task<bool> CourseExists(Guid courseId, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            return course != null;
        }
    }

    public class StudentAttendanceValidator : AbstractValidator<studentAttendanceDto>
    {
        public StudentAttendanceValidator()
        {
            RuleFor(x => x.studentId)
                .NotEmpty().WithMessage("Student ID is required");

            RuleFor(x => x.AttendanceState)
                .IsInEnum().WithMessage("Invalid attendance status");
        }
    }
}