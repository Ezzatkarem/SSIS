using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Fee
{
    public class CreateFeeDto
    {
        public Guid StudentId { get; set; }
        public int Semester { get; set; }
        public int AcademicYear { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
