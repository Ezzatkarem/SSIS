using FluentValidation;
using SSIS.BLL.DTOs.Fee;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Validators
{
    public class CreateFeeValidator : AbstractValidator<CreateFeeDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFeeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required")
                .MustAsync(StudentExists).WithMessage("Student does not exist");

            RuleFor(x => x.Semester)
                .NotEmpty().WithMessage("Semester is required");

            RuleFor(x => x.AcademicYear)
                .NotEmpty().WithMessage("Academic year is required");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than 0");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");
        }

        private async Task<bool> StudentExists(Guid studentId, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            return student != null;
        }
    }
}