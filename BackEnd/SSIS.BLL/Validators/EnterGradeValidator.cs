using FluentValidation;
using SSIS.BLL.DTOs.Grades;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Validators
{
    public class EnterGradeValidator : AbstractValidator<GradeDTO>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnterGradeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required")
                .MustAsync(StudentExists).WithMessage("Student does not exist");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required")
                .MustAsync(CourseExists).WithMessage("Course does not exist");

            RuleFor(x => x.Score)
                .InclusiveBetween(0, 100).WithMessage("Score must be between 0 and 100");

            RuleFor(x => x.Remarks)
                .MaximumLength(200).WithMessage("Remarks must be at most 200 characters");
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
    }
}