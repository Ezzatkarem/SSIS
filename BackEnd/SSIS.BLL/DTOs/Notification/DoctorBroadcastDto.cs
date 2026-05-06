using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Notification
{
    public class DoctorBroadcastDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public bool MyStudents { get; set; }        
        public Guid? CourseId { get; set; }         
        public Guid? StudentId { get; set; }       
        public int? AcademicLevel { get; set; }
    }
}
