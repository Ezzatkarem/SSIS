namespace SSIS.BLL.DTOs.Reports
{
    public class AtRiskStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public double GPA { get; set; }
        public double AttendancePercentage { get; set; }
        public string RiskLevel { get; set; } = "Low";
    }
}