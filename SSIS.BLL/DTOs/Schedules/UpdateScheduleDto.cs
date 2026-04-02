using SSIS.Domain.Enum;

namespace SSIS.BLL.DTOs.Schedules
{
    public class UpdateScheduleDto
    {
        public DayOfWeekEnum DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}
