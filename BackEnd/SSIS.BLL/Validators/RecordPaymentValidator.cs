using FluentValidation;
using SSIS.BLL.DTOs.Payment;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Validators
{
    public class RecordPaymentValidator : AbstractValidator<ManualPaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecordPaymentValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.feeId)
                .NotEmpty().WithMessage("Fee ID is required")
                .MustAsync(FeeExists).WithMessage("Fee does not exist");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.PaymentDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Payment date cannot be in the future");
        }

        private async Task<bool> FeeExists(Guid feeId, CancellationToken cancellationToken)
        {
            var fee = await _unitOfWork.Fees.GetByIdAsync(feeId);
            return fee != null;
        }
    }
}