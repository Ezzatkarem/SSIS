namespace SSIS.BLL.DTOs.Dashboard
{
    public class StudentDashboardDto
    {
        public double GPA { get; set; }
        public int CoursesCount { get; set; }
        public double AttendancePercentage { get; set; }
        public int UnreadNotifications { get; set; }
        public List<UpcomingScheduleDto> UpcomingSchedules { get; set; } = new();
        public List<RecentGradeDto> RecentGrades { get; set; } = new();
    }

    public class UpcomingScheduleDto
    {
        public Guid ScheduleId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}