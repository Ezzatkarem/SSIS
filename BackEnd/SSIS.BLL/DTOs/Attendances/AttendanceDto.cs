using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Attendances
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string? CourseName { get;  set; }
        public AttendanceState AttendanceState { get; set; }
        public DateTime CreateAt { get; set; }


    }
}
