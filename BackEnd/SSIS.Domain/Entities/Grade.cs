using System;
using System.Collections.Generic;
using System.Text;
using SSIS.Domain.Common;
using SSIS.Domain.Enum;


namespace SSIS.Domain.Entities
{
    public class Grade : BaseEntity,ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        
        public Guid GradeId { get; set; }
        public Guid StudentId {  get; set; }
        public Guid CourseId { get; set; }
        public decimal Score { get; set; }

        public GradeLetter GradeLetter { get; set; }

        public int Semester { get; set; }

        public int AcademicYear { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public User? Student { get; set; }
        public Course? Course { get; set; }

    }
}
