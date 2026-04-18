using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Grades
{
    public class GradeDTO
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public int academicYear { get; set; }
        public string Semester { get; set; }
       public GradeLetter GradeLetter { get; set; }
        public int Credits { get; set; }
        public string? Remarks { get; set; }
        public string CourseName { get; set; }

        public decimal Score { get; set; }

        public DateTime CreatedAt { get; set; }
    
    }
}
