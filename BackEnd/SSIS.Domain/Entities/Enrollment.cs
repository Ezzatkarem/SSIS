using SSIS.Domain.Common;
using System;

namespace SSIS.Domain.Entities
{
    public class Enrollment : BaseEntity, ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; } = true;

        public int Semester { get; set; }
        public int AcademicYear { get; set; }
        public decimal? SemesterGpa { get; set; }
        public int TotalCredits { get; set; }

        // Navigation properties
        public User Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
