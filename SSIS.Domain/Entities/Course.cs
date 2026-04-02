using SSIS.Domain.Common;
using System;

namespace SSIS.Domain.Entities
{
    public class Course : BaseEntity, ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public Guid? DoctorId { get; set; }
        public string Semester { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public User? Doctor { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
