namespace SSIS.BLL.DTOs.Courses
{
    public class UpdateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
