using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.DAL.Repositories
{
    public class PaymentRepo : Repository<Payment>, IpaymentRepo
    {
        public PaymentRepo(AppDbContext context) : base(context)
        {
        }
        

        public async Task<IReadOnlyList<Payment>> GetByFeeIdAsync(Guid FeeId)
        {
           return await _context.Payments
                .Where(f=>f.FeeId== FeeId)
                .OrderByDescending(f=>f.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetByPaymobOrderIdAsync(string OrderId)
        {
            return await _context.Payments
              .FirstOrDefaultAsync(f => f.PaymobOrderId == OrderId);
        }

        public async Task<IReadOnlyList<Payment>> GetByStudentIdAsync(Guid studentId)
        {
           return await _context.Payments
                .Where(f=>f.StudentId==studentId)
                .OrderByDescending(f=>f.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string TransactionId)
        {
            return await _context.Payments
              .FirstOrDefaultAsync(f => f.TransactionId == TransactionId);
        }

        public async Task<decimal> GetPaidFeesByStudentAsync(Guid studentId)
        {
           return await _context.Payments
                .Where(p=>p.StudentId==studentId &&p.PaymentStatus==PaymentStatus.Completed).SumAsync(p=>p.Amount);
        }
    }
}
