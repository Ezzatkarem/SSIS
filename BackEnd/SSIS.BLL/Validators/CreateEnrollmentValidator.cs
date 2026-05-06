using FluentValidation;
using SSIS.BLL.DTOs.Enrollments;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Validators
{
    public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateEnrollmentValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required")
                .MustAsync(StudentExists).WithMessage("Student does not exist");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required")
                .MustAsync(CourseExists).WithMessage("Course does not exist");

            RuleFor(x => x)
                .MustAsync(NotAlreadyEnrolled).WithMessage("Student is already enrolled in this course");
        }

        private async Task<bool> StudentExists(Guid studentId, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            return student != null;
        }

        private async Task<bool> CourseExists(Guid courseId, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            return course != null;
        }

        private async Task<bool> NotAlreadyEnrolled(CreateEnrollmentDto dto, CancellationToken cancellationToken)
        {
            return !await _unitOfWork.Enrollments.ExistsAsync(dto.StudentId, dto.CourseId);
        }
    }
}