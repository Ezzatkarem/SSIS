using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Payment
{
    public class ManualPaymentDto
    {
        public Guid feeId {  get; set; }
        public Guid StudentId { get; set; }
        public decimal Amount {  get; set; }
        public string PaymentMethod { get; set; }=string.Empty;
        public string? RefrenceNumber { get; set; }
        public DateTime PaymentDate { get;set; }
    }
}
