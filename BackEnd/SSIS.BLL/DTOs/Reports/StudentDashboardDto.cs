namespace SSIS.BLL.DTOs.Reports
{
    public class StudentDashboardDto
    {
        public decimal CurrentGpa { get; set; }
        public int EnrolledCoursesCount { get; set; }
        public decimal OverallAttendancePercentage { get; set; }
        public int UnreadNotifications { get; set; }
        public List<UpcomingFeeDto> UpcomingFees { get; set; } = new();
    }

    public class UpcomingFeeDto
    {
        public Guid FeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int Semester { get; set; }
        public int AcademicYear { get; set; }
    }
}