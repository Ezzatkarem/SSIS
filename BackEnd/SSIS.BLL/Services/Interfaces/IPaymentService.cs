using SSIS.BLL.DTOs.Payment;
using SSIS.BLL.Responce;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Responce<PaymentResponceDto>> InitiatePaymentAsync(InitiatePaymentDto dto, Guid StudentId);
        Task<Responce<PaymentResponceDto>> RecordManualPaymentAsync(ManualPaymentDto dto);
        Task<Responce<bool>> HandelPaymobCallbackAsync(PaymobCallBackDto dto);
        Task<Responce<PaymentResponceDto>> GetPaymentbyIdAsync( Guid PaymentId);
        Task<Responce<IReadOnlyList<PaymentResponceDto>>> GetPaymentsByStudentAsync(Guid studentId);



    }
}
