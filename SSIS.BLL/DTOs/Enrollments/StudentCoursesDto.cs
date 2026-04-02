namespace SSIS.BLL.DTOs.Enrollments
{
    public class StudentCoursesDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<CourseInfoDto> Courses { get; set; } = new List<CourseInfoDto>();
    }

    public class CourseInfoDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
