using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Interfaces
{
    public interface IFeeRepo :IRepository<Fee>
    {
        
        Task<IReadOnlyList<Fee>> GetByStudentIdAsync(Guid studentId);
        Task<IReadOnlyList<Fee>> GetBySTatusAsync(FeeStaus staus);
        Task<Fee?> GetByIdWithStudentAsync(Guid id);
        Task<IReadOnlyList<Fee>> GetOvardueFeesAsync(DateTime CurrentDate);
        Task<decimal> GetTotalFeesByStudentAsync(Guid studentId);
        Task<decimal> GetPaidFeesByStudentAsync(Guid studentId);




    }
}
