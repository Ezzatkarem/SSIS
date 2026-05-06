using FluentValidation;
using SSIS.BLL.DTOs.Grades;

namespace SSIS.BLL.Validators
{
    public class UpdateGradeValidator : AbstractValidator<UpdateGradeDTO>
    {
        public UpdateGradeValidator()
        {
            RuleFor(x => x.Score)
                .InclusiveBetween(0, 100).WithMessage("Score must be between 0 and 100");

            RuleFor(x => x.Remarks)
                .MaximumLength(200).WithMessage("Remarks must be at most 200 characters");
        }
    }
}