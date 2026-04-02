using SSIS.Domain.Common;
using SSIS.Domain.Enum;
using System;

namespace SSIS.Domain.Entities
{
    public class Schedule : BaseEntity, ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public Guid CourseId { get; set; }
        public DayOfWeekEnum DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;

        // Navigation property
        public Course Course { get; set; } = null!;
    }
}
