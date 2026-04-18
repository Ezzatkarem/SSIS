using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Payment
{
    public class InitiatePaymentDto
    {
        public Guid feeId { get; set; }
        public decimal Amount { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
