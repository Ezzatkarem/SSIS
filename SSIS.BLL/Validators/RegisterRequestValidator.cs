using FluentValidation;
using SSIS.Domain.Enum;
using SSIS.BLL.DTOs.Login;

namespace SSIS.BLL.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role");

            RuleFor(x => x.DocumentsFile)
                .NotNull().When(x => x.Role == UserRole.Student)
                .WithMessage("Documents file is required for students");
        }
    }
}