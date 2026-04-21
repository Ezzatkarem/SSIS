using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace SSIS.DAL.Repositories
{
    public class FeeRepo : Repository<Fee>, IFeeRepo
    {
        public FeeRepo(AppDbContext context) : base(context)
        {
        }


        public async Task<IReadOnlyList<Fee>> GetBySTatusAsync(FeeStaus status)
        {
            return await _context.Fees.Where(p => p.feeStatus == status).Include(p => p.Student).Include(p => p.Payments).ToListAsync();
        }
        public async Task<Fee?> GetByIdWithStudentAsync(Guid id)
        {
            return await _context.Fees
                .Include(f => f.Student)   
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IReadOnlyList<Fee>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Fees.Where(p=>p.StudentId==studentId).Include(p => p.Student).Include(p => p.Payments)
                .OrderByDescending(p=>p.academicYear)
                .ThenByDescending(p=>p.semester)
                .ToListAsync();

        }

        public async Task<IReadOnlyList<Fee>> GetOvardueFeesAsync(DateTime CurrentDate)
        {
            return await _context.Fees
                .Where(p => p.DueDate<CurrentDate &&p.feeStatus!=FeeStaus.Paid &&p.PaidAmount<p.TotalAmount)
                .Include(p => p.Student).Include(p => p.Payments).ToListAsync();

        }

        public async Task<decimal> GetTotalFeesByStudentAsync(Guid studentId)
        {
            return await _context.Fees.Where(p => p.StudentId==studentId).Include(p => p.Payments).SumAsync(p=>p.TotalAmount);

        }

        public async Task<decimal> GetPaidFeesByStudentAsync(Guid studentId)
        {
           return await _context.Fees
                .Where(p=>p.StudentId== studentId )
                .SumAsync(p=>p.PaidAmount);
        }
    }
}
