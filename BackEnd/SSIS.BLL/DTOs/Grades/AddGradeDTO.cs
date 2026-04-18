using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Grades
{
    public class AddGradeDTO
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public int AcademicYear { get; set; }
        public string Semester { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public decimal Score { get; set; }
    }
}
