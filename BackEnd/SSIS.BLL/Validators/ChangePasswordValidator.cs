using FluentValidation;
using SSIS.BLL.DTOs.Users;

namespace SSIS.BLL.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters");
        }
    }
}