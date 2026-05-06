using SSIS.BLL.DTOs.Reports;

namespace SSIS.BLL.Interfaces
{
    public interface IReportService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync(Guid adminId);
        Task<DoctorDashboardDto> GetDoctorDashboardAsync(Guid doctorId);
        Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId);
        Task<List<TopStudentDto>> GetTopStudentsAsync(int count);
        Task<CourseStatisticsDto> GetCourseStatisticsAsync(Guid courseId);
    }
}