namespace SSIS.BLL.DTOs.Reports
{
    public class TopStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public decimal Gpa { get; set; }
        public int TotalCredits { get; set; }
    }
}