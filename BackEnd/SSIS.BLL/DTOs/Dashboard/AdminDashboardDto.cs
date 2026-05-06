namespace SSIS.BLL.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalStudents { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalCourses { get; set; }
        public int ActiveEnrollments { get; set; }
        public double AverageAttendance { get; set; }
        public double AverageGrade { get; set; }
        public List<RecentGradeDto> RecentGrades { get; set; } = new();
        public List<TopStudentDto> TopStudents { get; set; } = new();
    }

    public class RecentGradeDto
    {
        public Guid GradeId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TopStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double GPA { get; set; }
        public int TotalCredits { get; set; }
    }
}