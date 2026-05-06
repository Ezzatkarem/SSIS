namespace SSIS.BLL.DTOs.Courses
{
    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public int Semester { get; set; } 
        public int AcademicYear { get; set; } 
    }
}
