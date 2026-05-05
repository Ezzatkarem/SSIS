using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Attendances
{
    public class TakeAttendanceDto
    {

        public Guid CourseId { get; set; }
        public DateTime Date { get; set; }
        public List<studentAttendanceDto> attendances { get; set; } = new();
    }
    public class studentAttendanceDto
    {
        public Guid studentId { get; set; }
        public AttendanceState AttendanceState { get; set; }
    }
}
