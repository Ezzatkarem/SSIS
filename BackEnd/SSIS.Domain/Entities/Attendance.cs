using SSIS.Domain.Common;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Entities
{
    public class Attendance :BaseEntity
    {
        public AttendanceState AttendanceState { get; set; }
        public DateTime Date { get; set; }


        // Navigation Properties

        public Guid StudentId { get; set; }
        public Guid courseId { get; set; }
        public User? Student { get; set; }
        public Course? course { get; set; }

    }
}
