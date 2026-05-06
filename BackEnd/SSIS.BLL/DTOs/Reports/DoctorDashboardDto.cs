using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Reports
{
    public class DoctorDashboardDto
    {
        public List<CourseStatisticsDto> course { get; set; } = new();
        public int totalstudent {  get; set; }
        public decimal AverageGrade { get; set; }
        public decimal AverageAttendance { get; set; }
        public int UnreadNotifications { get; set; }
    }
}
