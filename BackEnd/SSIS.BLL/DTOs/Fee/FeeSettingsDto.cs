using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Fee
{
    public class FeeSettingsDto
    {
        public int Semester {  get; set; }
        public int AcademicYear { get; set; }
        public decimal AmountPerStudent { get; set; }
        public DateTime DueDate { get; set; }
        public string? AcademicLevel { get; set; }


    }
}
