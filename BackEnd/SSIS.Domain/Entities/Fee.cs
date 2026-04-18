using SSIS.Domain.Common;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Entities
{
    public class Fee :BaseEntity
    {
        public Guid StudentId { get; set; }
        public int semester {  get; set; }
        public int academicYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        public FeeStaus feeStatus { get; set; } 


        public virtual List<Payment> Payments { get; set; }=new List<Payment>();
        public User Student { get; set; } = null!;

    }
}
