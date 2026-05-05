using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Attendances
{
    public class StudentAttendancePercentageDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double persentage { get; set; } 
    }
}
