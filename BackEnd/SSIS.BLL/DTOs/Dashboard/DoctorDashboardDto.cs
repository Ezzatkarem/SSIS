namespace SSIS.BLL.DTOs.Dashboard
{
    public class DoctorDashboardDto
    {
        public int CourseCount { get; set; }
        public int StudentCount { get; set; }
        public double AverageAttendance { get; set; }
        public double AverageGrade { get; set; }
        public GradeDistributionDto GradeDistribution { get; set; } = new();
        public List<RecentSubmissionDto> RecentSubmissions { get; set; } = new();
    }

    public class GradeDistributionDto
    {
        public int ACount { get; set; }
        public int BCount { get; set; }
        public int CCount { get; set; }
        public int DCount { get; set; }
        public int FCount { get; set; }
    }

    public class RecentSubmissionDto
    {
        public Guid GradeId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}