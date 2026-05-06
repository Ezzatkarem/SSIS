using FluentValidation;
using SSIS.BLL.DTOs.Courses;

namespace SSIS.BLL.Validators
{
    public class UpdateCourseValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .Length(3, 100).WithMessage("Course name must be between 3 and 100 characters");

            RuleFor(x => x.Credits)
                .InclusiveBetween(1, 10).WithMessage("Credits must be between 1 and 10");
        }
    }
}