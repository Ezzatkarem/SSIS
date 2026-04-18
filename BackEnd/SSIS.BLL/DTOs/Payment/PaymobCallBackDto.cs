using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Payment
{
    public class PaymobCallBackDto
    {
        public string Hmac { get; set; }=string.Empty;
        public PaymobCallBackData? Data { get; set; }

    }
    public class PaymobCallBackData
    {
        public string? OrderId { get; set; }
        public string? TransactionId { get; set; }
        public string? Amount { get; set; }
        public string? Success { get; set; }
        public string? Pending { get; set; }

    }

}
