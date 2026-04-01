using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using SSIS.PLL.DTOs.Login;


namespace SSIS.PLL.Validators
{
    public class LoginRequestValidator: AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
