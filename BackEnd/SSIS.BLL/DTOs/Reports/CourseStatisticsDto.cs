namespace SSIS.BLL.DTOs.Reports
{
    public class CourseStatisticsDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int EnrolledStudentsCount { get; set; }
        public decimal AverageGrade { get; set; }
        public decimal HighestGrade { get; set; }
        public decimal LowestGrade { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
}