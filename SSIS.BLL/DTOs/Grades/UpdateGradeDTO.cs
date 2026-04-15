using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Grades
{
    public class UpdateGradeDTO
    {
        public decimal Score { get; set; }
        public string? Remarks { get; set; }
    }
}
