using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Interfaces
{
    public interface IpaymentRepo :IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetByStudentIdAsync(Guid studentId);
        Task<IReadOnlyList<Payment>> GetByFeeIdAsync(Guid FeeId);
        Task<Payment?> GetByTransactionIdAsync(string TransactionId);
        Task<Payment?> GetByPaymobOrderIdAsync(string OrderId);
        Task<decimal> GetPaidFeesByStudentAsync(Guid studentId);

    }
}
