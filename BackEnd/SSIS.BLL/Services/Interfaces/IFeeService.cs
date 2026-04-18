using SSIS.BLL.DTOs.Fee;
using SSIS.BLL.Responce;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Interfaces
{
    public interface IFeeService
    {
        /// Admin
        Task<Responce<FeeResponceDto>> CreateFeeForStudentAsync(CreateFeeDto dto);
        Task<Responce<FeeResponceDto>> UpdateFeeAsync(Guid feeId, UpdateFee dto);
        Task<Responce<FeeResponceDto>> GetFeeByIdAsync(Guid feeId);
        Task<Responce<bool>> DeleteFeeAsync(Guid feeId);
        Task<Responce<IReadOnlyList<FeeResponceDto>>> GetAllFeesAsync ();
        Task<Responce<IReadOnlyList<FeeResponceDto>>> GetFeesByStudentAsync (Guid studentId);

        ///// Student
        Task<Responce<IReadOnlyList<FeeResponceDto>>> GetMyFeesAsync(Guid studentId);

        /// Background Job
        Task AutoGenerateFeesAsync (FeeSettingsDto dto);


    }
}
