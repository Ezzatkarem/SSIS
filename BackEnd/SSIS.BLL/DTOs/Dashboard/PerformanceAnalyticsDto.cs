namespace SSIS.BLL.DTOs.Dashboard
{
    public class PerformanceAnalyticsDto
    {
        public List<TrendPointDto> GradeTrends { get; set; } = new();
        public List<TrendPointDto> AttendanceTrends { get; set; } = new();
        public string RiskLevel { get; set; } = "Low";
    }

    public class TrendPointDto
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}