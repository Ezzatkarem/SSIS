using FluentValidation;
using SSIS.BLL.DTOs.Courses;

namespace SSIS.BLL.Validators
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .Length(3, 100).WithMessage("Course name must be between 3 and 100 characters");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Course code is required")
                .Matches(@"^[A-Za-z]{2,4}\d{3,4}$").WithMessage("Course code must follow format like 'CS101'");

            RuleFor(x => x.Credits)
                .InclusiveBetween(1, 10).WithMessage("Credits must be between 1 and 10");

            RuleFor(x => x.Semester)
                .NotEmpty().WithMessage("Semester is required");

            RuleFor(x => x.AcademicYear)
                .NotEmpty().WithMessage("Academic year is required");
        }
    }
}