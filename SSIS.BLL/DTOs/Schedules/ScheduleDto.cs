using SSIS.Domain.Enum;

namespace SSIS.BLL.DTOs.Schedules
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DayOfWeekEnum DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}
