namespace SSIS.BLL.DTOs.Reports
{
    public class CourseGradeStatisticsDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public double MinScore { get; set; }
        public double MaxScore { get; set; }
        public GradeCountDto GradeDistribution { get; set; } = new();
    }

    public class GradeCountDto
    {
        public int ACount { get; set; }
        public int BCount { get; set; }
        public int CCount { get; set; }
        public int DCount { get; set; }
        public int FCount { get; set; }
    }
}