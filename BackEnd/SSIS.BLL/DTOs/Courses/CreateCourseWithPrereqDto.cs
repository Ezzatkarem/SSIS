using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Courses
{
    public class CreateCourseWithPrereqDto
    {
        public string Name { get; set; }=string.Empty;
        public string Code { get; set; }=string.Empty;
        public int Credits { get; set; }
        public string? Descreption { get; set; }
        public Guid ? DoctorID { get; set; }
        public int semester {  get; set; }
        public int AcedemicYear { get; set; }
        public List<Guid> PrerequistisIds { get; set; } = new();
    }
}
