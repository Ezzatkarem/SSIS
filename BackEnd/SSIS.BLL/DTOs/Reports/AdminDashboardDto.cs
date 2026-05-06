using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Reports
{
    public class AdminDashboardDto
    {
        public int TotalStudent {  get; set; }
        public int TotalCourse { get; set; }
        public int TotalDoctor { get; set; }
        public int TotalEnrollment { get; set; }
        public decimal TotalFeesCollected { get; set; }
        public decimal TotalFeesPending { get; set; }
        public int OverdueFeesCount { get; set; }
        public decimal AverageGpa { get; set; }
        public decimal AverageAttendance { get; set; }
        public List<TopStudentDto> TopStudents { get; set; } = new();
        public int UnreadNotifications { get; set; }
    }
}
