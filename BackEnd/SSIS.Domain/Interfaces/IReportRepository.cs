using SSIS.Domain.Entities;

namespace SSIS.Domain.Interfaces
{
    public interface IReportRepository
    {
        // Admin Dashboard 
        Task<int> GetTotalStudentsAsync();
        Task<int> GetTotalDoctorsAsync();
        Task<int> GetTotalCoursesAsync();
        Task<int> GetTotalEnrollmentsAsync();
        Task<decimal> GetTotalFeesCollectedAsync();
        Task<decimal> GetTotalFeesPendingAsync();
        Task<int> GetOverdueFeesCountAsync(DateTime currentDate);
        Task<decimal> GetAverageAttendanceAsync();

        Task<List<(Guid StudentId, string StudentName, decimal Gpa, int TotalCredits)>> GetTopStudentsAsync(int count);

        // Doctor Dashboard
        Task<List<(Guid CourseId, string CourseName, string CourseCode, int EnrolledCount,
       decimal AvgGrade, decimal HighestGrade, decimal LowestGrade, decimal AttendancePct)>>GetDoctorCourseStatisticsAsync(Guid doctorId);

        // Student Dashboard
        Task<User?> GetUserWithGpaAsync(Guid studentId);
        Task<int> GetEnrolledCoursesCountAsync(Guid studentId);
        Task<decimal> GetStudentAttendancePercentageAsync(Guid studentId);
        Task<List<Fee>> GetUpcomingFeesAsync(Guid studentId);
    }
}