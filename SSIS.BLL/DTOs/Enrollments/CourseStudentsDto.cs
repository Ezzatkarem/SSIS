namespace SSIS.BLL.DTOs.Enrollments
{
    public class CourseStudentsDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public List<StudentInfoDto> Students { get; set; } = new List<StudentInfoDto>();
    }

    public class StudentInfoDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }
}
