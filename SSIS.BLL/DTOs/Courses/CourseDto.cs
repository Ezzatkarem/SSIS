namespace SSIS.BLL.DTOs.Courses
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string Semester { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
