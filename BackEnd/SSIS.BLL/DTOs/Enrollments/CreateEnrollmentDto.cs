namespace SSIS.BLL.DTOs.Enrollments
{
    public class CreateEnrollmentDto
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
    }
}
