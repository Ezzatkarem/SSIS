using SSIS.BLL.DTOs.Dashboard;
using SSIS.BLL.DTOs.Reports;
using SSIS.BLL.Responce;

namespace SSIS.BLL.Interfaces
{
    public interface IReportService
    {
        Task<Responce<AdminDashboardDto>> GetAdminDashboardAsync();
        Task<Responce<DoctorDashboardDto>> GetDoctorDashboardAsync(Guid doctorId);
        Task<Responce<StudentDashboardDto>> GetStudentDashboardAsync(Guid studentId);
        Task<Responce<CourseGradeStatisticsDto>> GetCourseGradeStatisticsAsync(Guid courseId);
        Task<Responce<List<AtRiskStudentDto>>> GetAtRiskStudentsAsync();
        Task<Responce<PerformanceAnalyticsDto>> GetStudentAnalyticsAsync(Guid studentId);
    }
}