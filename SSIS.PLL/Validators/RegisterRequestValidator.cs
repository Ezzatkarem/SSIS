using FluentValidation;
using Microsoft.Extensions.Configuration;
using SSIS.Domain.Enum;
using SSIS.PLL.DTOs.Login;

namespace SSIS.PLL.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        private readonly IConfiguration _configuration;

        public RegisterRequestValidator(IConfiguration configuration)
        {
            _configuration = configuration;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            When(x => x.Role == UserRole.Student, () =>
            {
                RuleFor(x => x.SecondarySchoolCertificate)
                    .NotNull().WithMessage("Secondary school certificate is required for students");
            });

            When(x => x.Role == UserRole.Doctor, () =>
            {
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Title is required");
                RuleFor(x => x.Specialization)
                    .NotEmpty().WithMessage("Specialization is required");
                RuleFor(x => x.YearsOfExperience)
                    .NotNull().WithMessage("Years of experience is required")
                    .InclusiveBetween(0, 50).WithMessage("Years of experience must be between 0 and 50");
                RuleFor(x => x.UniversityDegree)
                    .NotNull().WithMessage("University degree certificate is required for doctors");
            });

            When(x => x.Role == UserRole.Admin, () =>
            {
                var adminCode = _configuration["AdminSettings:RegistrationCode"];
                RuleFor(x => x.AdminCode)
                    .NotEmpty().WithMessage("Admin code is required")
                    .Equal(adminCode).WithMessage("Invalid admin code");
            });

            RuleFor(x => x.NationalIdImage)
                .NotNull().WithMessage("National ID image is required");
        }
    }
}