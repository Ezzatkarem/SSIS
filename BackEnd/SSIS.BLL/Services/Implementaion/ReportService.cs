using SSIS.BLL.DTOs.Reports;
using SSIS.BLL.Interfaces;
using SSIS.BLL.Services.Interfaces;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Services.Implementaion
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepo;
        private readonly INotificationService _notificationService;

        public ReportService(IReportRepository reportRepo, INotificationService notificationService)
        {
            _reportRepo = reportRepo;
            _notificationService = notificationService;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid adminId)
        {
            var unreadCount = await _notificationService.GetUnReadCountAsync(adminId);
            var topStudentsRaw = await _reportRepo.GetTopStudentsAsync(10);

            var topStudents = topStudentsRaw.Select(s => new TopStudentDto
            {
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                Gpa = s.Gpa,
                TotalCredits = s.TotalCredits
            }).ToList();

            var dashboard = new AdminDashboardDto
            {
                TotalStudent = await _reportRepo.GetTotalStudentsAsync(),
                TotalDoctor = await _reportRepo.GetTotalDoctorsAsync(),
                TotalCourse = await _reportRepo.GetTotalCoursesAsync(),
                TotalEnrollment = await _reportRepo.GetTotalEnrollmentsAsync(),
                TotalFeesCollected = await _reportRepo.GetTotalFeesCollectedAsync(),
                TotalFeesPending = await _reportRepo.GetTotalFeesPendingAsync(),
                OverdueFeesCount = await _reportRepo.GetOverdueFeesCountAsync(DateTime.UtcNow),
                AverageAttendance = await _reportRepo.GetAverageAttendanceAsync(),
                TopStudents = topStudents,
                UnreadNotifications = unreadCount
            };

            return dashboard;
        }

        public async Task<DoctorDashboardDto> GetDoctorDashboardAsync(Guid doctorId)
        {
            var coursesRaw = await _reportRepo.GetDoctorCourseStatisticsAsync(doctorId);
            var unreadCount = await _notificationService.GetUnReadCountAsync(doctorId);

            var courses = coursesRaw.Select(c => new CourseStatisticsDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                CourseCode = c.CourseCode,
                EnrolledStudentsCount = c.EnrolledCount,
                AverageGrade = c.AvgGrade,
                HighestGrade = c.HighestGrade,
                LowestGrade = c.LowestGrade,
                AttendancePercentage = c.AttendancePct
            }).ToList();

            return new DoctorDashboardDto
            {
                course = courses,
                totalstudent = courses.Sum(c => c.EnrolledStudentsCount),
                AverageGrade = courses.Any() ? courses.Average(c => c.AverageGrade) : 0,
                AverageAttendance = courses.Any() ? courses.Average(c => c.AttendancePercentage) : 0,
                UnreadNotifications = unreadCount
            };
        }

        public async Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId)
        {
            var user = await _reportRepo.GetUserWithGpaAsync(studentId);
            var enrolledCount = await _reportRepo.GetEnrolledCoursesCountAsync(studentId);
            var attendancePct = await _reportRepo.GetStudentAttendancePercentageAsync(studentId);
            var unreadCount = await _notificationService.GetUnReadCountAsync(studentId);
            var upcomingFeesRaw = await _reportRepo.GetUpcomingFeesAsync(studentId);

            var upcomingFees = upcomingFeesRaw.Select(f => new UpcomingFeeDto
            {
                FeeId = f.Id,
                Amount = f.TotalAmount - f.PaidAmount,
                DueDate = f.DueDate,
                Semester = f.semester,
                AcademicYear = f.academicYear
            }).ToList();

            return new StudentDashboardDto
            {
                CurrentGpa = user?.ComulativeGpa ?? 0,
                EnrolledCoursesCount = enrolledCount,
                OverallAttendancePercentage = Math.Round(attendancePct, 2),
                UnreadNotifications = unreadCount,
                UpcomingFees = upcomingFees
            };
        }

        public async Task<List<TopStudentDto>> GetTopStudentsAsync(int count)
        {
            var raw = await _reportRepo.GetTopStudentsAsync(count);
            return raw.Select(s => new TopStudentDto
            {
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                Gpa = s.Gpa,
                TotalCredits = s.TotalCredits
            }).ToList();
        }

        public async Task<CourseStatisticsDto> GetCourseStatisticsAsync(Guid courseId)
        {
            var coursesRaw = await _reportRepo.GetDoctorCourseStatisticsAsync(Guid.Empty);
            var courseRaw = coursesRaw.FirstOrDefault(c => c.CourseId == courseId);

            if (courseRaw == default)
                return new CourseStatisticsDto();

            return new CourseStatisticsDto
            {
                CourseId = courseRaw.CourseId,
                CourseName = courseRaw.CourseName,
                CourseCode = courseRaw.CourseCode,
                EnrolledStudentsCount = courseRaw.EnrolledCount,
                AverageGrade = courseRaw.AvgGrade,
                HighestGrade = courseRaw.HighestGrade,
                LowestGrade = courseRaw.LowestGrade,
                AttendancePercentage = courseRaw.AttendancePct
            };
        }
    }
}