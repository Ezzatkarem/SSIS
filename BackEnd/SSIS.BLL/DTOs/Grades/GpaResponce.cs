using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Grades
{
    public class GpaResponce
    {
        public decimal SemesterGpa { get; set; }
        public decimal CumulativeGpa { get; set; }
        public int TotalCredits { get; set; }
        public int Semester { get; set; }
        public int AcademicYear { get; set; }

    }
}
