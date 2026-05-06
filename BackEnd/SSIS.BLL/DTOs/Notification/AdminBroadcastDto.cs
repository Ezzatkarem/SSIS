using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Notification
{
    public class AdminBroadcastDto
    {
        public string Title { get; set; }=string .Empty;
        public string Message {  get; set; }    =string .Empty;
        public int ?AcademicLevel { get; set; }

        public Guid? CourceId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? DoctorID { get; set; }
        public bool AllStudents { get; set; }
        public bool AllDoctors { get; set; }
        public bool AllUsers { get; set; }

    }
}
