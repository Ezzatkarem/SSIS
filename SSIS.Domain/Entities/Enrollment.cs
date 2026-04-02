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

        // Navigation properties
        public User Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
