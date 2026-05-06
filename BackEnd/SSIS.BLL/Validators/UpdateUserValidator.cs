using FluentValidation;
using SSIS.BLL.DTOs.Users;

namespace SSIS.BLL.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserValidator()
        {
            When(x => !string.IsNullOrEmpty(x.FullName), () =>
            {
                RuleFor(x => x.FullName)
                    .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters");
            });

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber)
                    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be in valid international format");
            });
        }
    }
}