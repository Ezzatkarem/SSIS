using SSIS.BLL.DTOs.Payment;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Fee
{
    public class FeeResponceDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }=string.Empty;
        public int Semester {  get; set; }
        public int AcademicYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ReminingAmount => TotalAmount-PaidAmount;
        public DateTime DueDate { get; set; }
        public FeeStaus FeeStaus { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PaymentResponceDto> Payments { get; set; } = new();


    }
}
