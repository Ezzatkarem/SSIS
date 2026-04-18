using SSIS.Domain.Enum;

namespace SSIS.BLL.DTOs.Schedules
{
    public class CreateScheduleDto
    {
        public Guid CourseId { get; set; }
        public DayOfWeekEnum DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}
