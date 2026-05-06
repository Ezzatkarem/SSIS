using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.DAL.Repositories
{
    public class ReportReposatory : IReportRepository
    {
        private readonly AppDbContext context;

        public ReportReposatory(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<decimal> GetAverageAttendanceAsync()
        {
            var res= await context.Attendances.ToListAsync();
            var persent = res.Count(p => p.AttendanceState == AttendanceState.Present);
            return (decimal)persent/res.Count()*100;
            
        }

        public async Task<List<(Guid CourseId, string CourseName, string CourseCode, int EnrolledCount,
                     decimal AvgGrade, decimal HighestGrade, decimal LowestGrade, decimal AttendancePct)>>
             GetDoctorCourseStatisticsAsync(Guid doctorId)
        {
            var courses = await context.Courses
                .Where(c => c.DoctorId == doctorId && !c.IsDeleted)
                .ToListAsync();

            var result = new List<(Guid, string, string, int, decimal, decimal, decimal, decimal)>();

            foreach (var course in courses)
            {
                var enrollments = await context.Enrollments
                    .Where(e => e.CourseId == course.Id && e.IsActive)
                    .CountAsync();

                var grades = await      context.Grades
                    .Where(g => g.CourseId == course.Id)
                    .ToListAsync();

                var avgGrade = grades.Any() ? grades.Average(g => g.Score) : 0;
                var highestGrade = grades.Any() ? grades.Max(g => g.Score) : 0;
                var lowestGrade = grades.Any() ? grades.Min(g => g.Score) : 0;

                var attendances = await context.Attendances
                    .Where(a => a.courseId == course.Id)
                    .ToListAsync();
                var attendancePct = attendances.Any()
                    ? (decimal)attendances.Count(a => a.AttendanceState == AttendanceState.Present) / attendances.Count * 100
                    : 0;

                result.Add((course.Id, course.Name, course.Code, enrollments,
                    Math.Round(avgGrade, 2), Math.Round(highestGrade, 2), Math.Round(lowestGrade, 2), Math.Round(attendancePct, 2)));
            }

            return result;
        }


        public async Task<int> GetEnrolledCoursesCountAsync(Guid studentId)
        {
            return await context.Enrollments
                           .CountAsync(e => e.StudentId == studentId && e.IsActive);
        }

        public async Task<int> GetOverdueFeesCountAsync(DateTime currentDate)
        {
            return await context.Fees
                .CountAsync(f => f.DueDate < currentDate
                              && f.PaidAmount < f.TotalAmount
                              && f.feeStatus != FeeStaus.Paid);
        }
        public async Task<decimal> GetStudentAttendancePercentageAsync(Guid studentId)
        {
            var attendances = await context.Attendances
                .Where(a => a.StudentId == studentId)
                .ToListAsync();

            if (!attendances.Any()) return 0;

            var present = attendances.Count(a => a.AttendanceState == AttendanceState.Present);
            return (decimal)present / attendances.Count * 100;
        }

        public async Task<List<(Guid StudentId, string StudentName, decimal Gpa, int TotalCredits)>> GetTopStudentsAsync(int count)
        {
            var students = await context.Users
                .Where(u => u.Role == UserRole.Student && !u.IsDeleted)
                .Select(u => new { u.Id, u.FullName, u.ComulativeGpa, u.TotalCompletedCredits })
                .ToListAsync();

            return students
                .OrderByDescending(s => s.ComulativeGpa)
                .Take(count)
                .Select(s => (s.Id, s.FullName, s.ComulativeGpa ?? 0, s.TotalCompletedCredits))
                .ToList();
        }


        public async Task<int> GetTotalCoursesAsync()
        {
            return await context.Courses.CountAsync(p=>!p.IsDeleted);
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            return await context.Users.CountAsync(p => p.Role == UserRole.Doctor && !p.IsDeleted);
        }

        public async Task<int> GetTotalEnrollmentsAsync()
        {
            return await context.Enrollments.CountAsync();
        }

        public async Task<decimal> GetTotalFeesCollectedAsync()
        {
            return await context.Payments
                           .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                           .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetTotalFeesPendingAsync()
        {
            var fees = await context.Fees.ToListAsync();
            return fees.Sum(f => f.TotalAmount - f.PaidAmount);
        }
        public async Task<int> GetTotalStudentsAsync()
        {
            return await context.Users
         .CountAsync(u => u.Role == UserRole.Student && !u.IsDeleted);
        }

        public async Task<List<Fee>> GetUpcomingFeesAsync(Guid studentId)
        {
            return await context.Fees
                .Where(f => f.StudentId == studentId && f.PaidAmount < f.TotalAmount && f.DueDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithGpaAsync(Guid studentId)
        {
            return await context.Users
                .Where(u => u.Id == studentId && u.Role == UserRole.Student && !u.IsDeleted)
                .Select(u => new User
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    ComulativeGpa = u.ComulativeGpa,
                    TotalCompletedCredits = u.TotalCompletedCredits
                })
                .FirstOrDefaultAsync();
        }
    }
}
