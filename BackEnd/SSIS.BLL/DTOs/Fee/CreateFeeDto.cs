using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Fee
{
    public class CreateFeeDto
    {
        public int? AcademicYear { get; set; }
        public int? Semester {  get; set; }
        public int? AcademicLevel { get; set; }
        public Guid StudentId { get; set; }
        public decimal? NewTotalAmount { get; set; }
        public DateTime? NewDueDate { get; set; }
        public string? Reason { get; set; }=string.Empty;
    }
}
