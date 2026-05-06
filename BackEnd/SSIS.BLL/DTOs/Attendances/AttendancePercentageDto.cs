using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Attendances
{
    public class AttendancePercentageDto
    {
        public Guid courseId {  get; set; }
        public string courseName { get; set; }=string.Empty;
        public double persentage { get; set; } 
    }
}
