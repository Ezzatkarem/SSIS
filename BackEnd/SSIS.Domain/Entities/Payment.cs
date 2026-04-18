using SSIS.Domain.Common;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Entities
{
    public class Payment :BaseEntity
    {
        public Guid FeeId { get; set; }
        public  Guid StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime  PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }=string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public string? ReceipeUrl { get; set; }
        public string? PaymobOrderId { get; set; }


        public virtual Fee Fee { get; set; } = null!;
        public virtual User Student { get; set; } = null!;

    }
}
