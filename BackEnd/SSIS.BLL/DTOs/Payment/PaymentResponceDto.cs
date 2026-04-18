using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Payment
{
    public class PaymentResponceDto
    {
        public Guid Id { get; set; }
        public Guid feeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }=string.Empty;
        public PaymentStatus PaymentStatus {  get; set; }
        public string? TransactionId { get; set; }
        public string? ReceipId { get; set; }
    }
}
