using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SSIS.BLL.DTOs.Dashboard;
using SSIS.BLL.DTOs.Reports;
using SSIS.BLL.Interfaces;
using SSIS.BLL.Responce;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Responce<AdminDashboardDto>> GetAdminDashboardAsync()
        {
            var totalStudents = (await _unitOfWork.Users.GetAllAsync()).Count(u => u.Role == UserRole.Student && !u.IsDeleted);
            var totalDoctors = (await _unitOfWork.Users.GetAllAsync()).Count(u => u.Role == UserRole.Doctor && !u.IsDeleted);
            var totalCourses = (await _unitOfWork.Courses.GetAllAsync()).Count(c => !c.IsDeleted);
            var activeEnrollments = (await _unitOfWork.Enrollments.GetAllAsync()).Count;

            var allGrades = await _context.Grades.ToListAsync();
            var avgGrade = allGrades.Any() ? (double)allGrades.Average(g => (decimal)g.Score) : 0;

            var recentGrades = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .OrderByDescending(g => g.CreatedAt)
                .Take(10)
                .Select(g => new RecentGradeDto
                {
                    GradeId = g.Id,
                    StudentName = g.Student.FullName,
                    CourseName = g.Course.Name,
                    Score = g.Score,
                    CreatedAt = g.CreatedAt
                })
                .ToListAsync();

            var topStudents = await _context.Grades
                .Include(g => g.Student)
                .GroupBy(g => g.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    StudentName = g.First().Student.FullName,
                    AvgScore = g.Average(x => (double)x.Score),
                    TotalCredits = g.Sum(x => x.Course.Credits)
                })
                .OrderByDescending(s => s.AvgScore)
                .Take(10)
                .ToListAsync();

            var dashboard = new AdminDashboardDto
            {
                TotalStudents = totalStudents,
                TotalDoctors = totalDoctors,
                TotalCourses = totalCourses,
                ActiveEnrollments = activeEnrollments,
                AverageAttendance = 0,
                AverageGrade = Math.Round(avgGrade, 2),
                RecentGrades = recentGrades,
                TopStudents = topStudents.Select(s => new TopStudentDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    GPA = Math.Round(s.AvgScore / 25.0, 2),
                    TotalCredits = s.TotalCredits
                }).ToList()
            };

            return new Responce<AdminDashboardDto>(dashboard, true, null!);
        }

        public async Task<Responce<DoctorDashboardDto>> GetDoctorDashboardAsync(Guid doctorId)
        {
            var courses = (await _unitOfWork.Courses.GetAllAsync()).Where(c => c.DoctorId == doctorId && !c.IsDeleted).ToList();
            var courseIds = courses.Select(c => c.Id).ToList();

            var enrollments = await _context.Enrollments.Where(e => courseIds.Contains(e.CourseId)).ToListAsync();
            var studentCount = enrollments.Select(e => e.StudentId).Distinct().Count();

            var grades = await _context.Grades.Where(g => courseIds.Contains(g.CourseId)).ToListAsync();
            var avgGrade = grades.Any() ? (double)grades.Average(g => (decimal)g.Score) : 0;

            var distribution = new GradeDistributionDto
            {
                ACount = grades.Count(g => g.GradeLetter == GradeLetter.A),
                BCount = grades.Count(g => g.GradeLetter == GradeLetter.B),
                CCount = grades.Count(g => g.GradeLetter == GradeLetter.C),
                DCount = grades.Count(g => g.GradeLetter == GradeLetter.D),
                FCount = grades.Count(g => g.GradeLetter == GradeLetter.F)
            };

            var recentSubmissions = await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .Where(g => courseIds.Contains(g.CourseId))
                .OrderByDescending(g => g.CreatedAt)
                .Take(10)
                .Select(g => new RecentSubmissionDto
                {
                    GradeId = g.Id,
                    StudentName = g.Student.FullName,
                    CourseName = g.Course.Name,
                    Score = g.Score,
                    CreatedAt = g.CreatedAt
                })
                .ToListAsync();

            var dashboard = new DoctorDashboardDto
            {
                CourseCount = courses.Count,
                StudentCount = studentCount,
                AverageAttendance = 0,
                AverageGrade = Math.Round(avgGrade, 2),
                GradeDistribution = distribution,
                RecentSubmissions = recentSubmissions
            };

            return new Responce<DoctorDashboardDto>(dashboard, true, null!);
        }

        public async Task<Responce<StudentDashboardDto>> GetStudentDashboardAsync(Guid studentId)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            if (student == null)
                return new Responce<StudentDashboardDto>(null!, false, "Student not found");

            var enrollments = await _context.Enrollments.Where(e => e.StudentId == studentId).ToListAsync();
            var courseIds = enrollments.Select(e => e.CourseId).ToList();

            var grades = await _context.Grades.Where(g => g.StudentId == studentId).ToListAsync();
            var gpa = grades.Any()
                ? Math.Round(grades.Average(g => (double)g.Score) / 25.0, 2)
                : 0.0;

            var schedules = await _context.Schedules
                .Include(s => s.Course)
                .Where(s => courseIds.Contains(s.CourseId))
                .ToListAsync();

            var upcomingSchedules = schedules.Select(s => new UpcomingScheduleDto
            {
                ScheduleId = s.Id,
                CourseName = s.Course.Name,
                DayOfWeek = s.DayOfWeek.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Room = s.Room
            }).ToList();

            var recentGrades = grades
                .OrderByDescending(g => g.CreatedAt)
                .Take(5)
                .Join(_context.Users, g => g.StudentId, u => u.Id, (g, u) => new { g, u })
                .Join(_context.Courses, x => x.g.CourseId, c => c.Id, (x, c) => new RecentGradeDto
                {
                    GradeId = x.g.Id,
                    StudentName = x.u.FullName,
                    CourseName = c.Name,
                    Score = x.g.Score,
                    CreatedAt = x.g.CreatedAt
                })
                .ToList();

            var dashboard = new StudentDashboardDto
            {
                GPA = gpa,
                CoursesCount = enrollments.Count,
                AttendancePercentage = 0,
                UnreadNotifications = 0,
                UpcomingSchedules = upcomingSchedules,
                RecentGrades = recentGrades
            };

            return new Responce<StudentDashboardDto>(dashboard, true, null!);
        }

        public async Task<Responce<CourseGradeStatisticsDto>> GetCourseGradeStatisticsAsync(Guid courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null)
                return new Responce<CourseGradeStatisticsDto>(null!, false, "Course not found");

            var grades = await _context.Grades.Where(g => g.CourseId == courseId).ToListAsync();

            if (!grades.Any())
                return new Responce<CourseGradeStatisticsDto>(null!, false, "No grades found for this course");

            var stats = new CourseGradeStatisticsDto
            {
                CourseId = courseId,
                CourseName = course.Name,
                AverageScore = Math.Round((double)grades.Average(g => (decimal)g.Score), 2),
                MinScore = (double)grades.Min(g => (decimal)g.Score),
                MaxScore = (double)grades.Max(g => (decimal)g.Score),
                GradeDistribution = new GradeCountDto
                {
                    ACount = grades.Count(g => g.GradeLetter == GradeLetter.A),
                    BCount = grades.Count(g => g.GradeLetter == GradeLetter.B),
                    CCount = grades.Count(g => g.GradeLetter == GradeLetter.C),
                    DCount = grades.Count(g => g.GradeLetter == GradeLetter.D),
                    FCount = grades.Count(g => g.GradeLetter == GradeLetter.F)
                }
            };

            return new Responce<CourseGradeStatisticsDto>(stats, true, null!);
        }

        public async Task<Responce<List<AtRiskStudentDto>>> GetAtRiskStudentsAsync()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

            var atRiskStudents = new List<AtRiskStudentDto>();

            foreach (var enrollment in enrollments.Where(e => !e.Student.IsDeleted))
            {
                var grades = await _context.Grades
                    .Where(g => g.StudentId == enrollment.StudentId && g.CourseId == enrollment.CourseId)
                    .ToListAsync();

                var gpa = grades.Any()
                    ? Math.Round(grades.Average(g => (double)g.Score) / 25.0, 2)
                    : 0.0;

                var riskLevel = gpa < 1.0 ? "High" : gpa < 2.0 ? "Medium" : "Low";

                if (riskLevel != "Low")
                {
                    atRiskStudents.Add(new AtRiskStudentDto
                    {
                        StudentId = enrollment.StudentId,
                        StudentName = enrollment.Student.FullName,
                        CourseName = enrollment.Course.Name,
                        GPA = gpa,
                        AttendancePercentage = 0,
                        RiskLevel = riskLevel
                    });
                }
            }

            return new Responce<List<AtRiskStudentDto>>(atRiskStudents, true, null!);
        }

        public async Task<Responce<PerformanceAnalyticsDto>> GetStudentAnalyticsAsync(Guid studentId)
        {
            var grades = await _context.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Semester)
                .ThenBy(g => g.CreatedAt)
                .ToListAsync();

            var gradeTrends = grades
                .GroupBy(g => new { g.Semester, g.Course.AcademicYear })
                .Select(g => new TrendPointDto
                {
                    Label = $"Semester {g.Key.Semester} - {g.Key.AcademicYear}",
                    Value = Math.Round(g.Average(x => (double)x.Score), 2)
                })
                .ToList();

            var analytics = new PerformanceAnalyticsDto
            {
                GradeTrends = gradeTrends,
                AttendanceTrends = new List<TrendPointDto>(),
                RiskLevel = grades.Any() && grades.Average(g => (double)g.Score) < 50 ? "High" : "Low"
            };

            return new Responce<PerformanceAnalyticsDto>(analytics, true, null!);
        }
    }
}