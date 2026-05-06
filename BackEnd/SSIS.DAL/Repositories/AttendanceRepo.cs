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
    public class AttendanceRepo : Repository<Attendance>, IAttendaceRepo
    {
        public AttendanceRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<Dictionary<Guid, double>> GetAttendancePercentageByCourseAsync(Guid studentId)
        {
            var attendaces=await _context.Attendances.Where(a=>a.StudentId==studentId).ToListAsync();
            var result = new Dictionary<Guid, double>();

            foreach (var courseGroup in attendaces.GroupBy(a => a.courseId))
            {
                var total = courseGroup.Count();
                var present = courseGroup.Count(a => a.AttendanceState == AttendanceState.Present);
                var percentage = total == 0 ? 0 : (double)present / total * 100;
                result.Add(courseGroup.Key, percentage);
            }

            return result;
        }

        public async Task<double> GetOveerAllAttendancePercentageForCourseAsync(Guid courseId)
        {
            var attendaces = await _context.Attendances.Where(a => a.courseId == courseId).ToListAsync();
            var result = new Dictionary<Guid, double>();

           
                var total = attendaces.Count();
                var present = attendaces.Count(a => a.AttendanceState == AttendanceState.Present);
                var percentage = total == 0 ? 0 : (double)present / total * 100;



            return Math.Round(percentage, 2);
        }

        public async Task< double> GetOverAllAttendancePercentageForStudentAsync(Guid StudentId)
        {
            var attendaces = await _context.Attendances.Where(a => a.StudentId == StudentId).ToListAsync();
            var result = new Dictionary<Guid, double>();


            var total = attendaces.Count();
            var present = attendaces.Count(a => a.AttendanceState == AttendanceState.Present);
            var percentage = total == 0 ? 0 : (double)present / total * 100;



            return Math.Round(percentage, 2);
        }

        public async Task<IReadOnlyList<Attendance>> GetByCourseIdAsync(Guid courseid)
        {
            return await _context.Attendances
                .Include(p=>p.Student)
                .Where(p => p.courseId == courseid)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task<Attendance?> GetByStudentCourseAndDateAsync(Guid studentId, Guid courseId, DateOnly date)
        {
           return await _context.Attendances
                .FirstOrDefaultAsync(p=>p.courseId==courseId&&p.StudentId==studentId
              && p.Date.Date == date.ToDateTime(TimeOnly.MinValue).Date);  
        }

        public async Task<IReadOnlyList<Attendance>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Attendances.
                Include(c=>c.course)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p=>p.Date)
                .ToListAsync();
        }

        public async Task<int> GetConsecutiveAbsencesAsync(Guid studentId, Guid courseId)
        {
            var attendaces = await _context.Attendances
                .Where(a => a.courseId == courseId&&a.StudentId==a.StudentId)
                .OrderByDescending(p=>p.Date).ToListAsync();



            var ConsecutiveAbsences = 0;
            foreach (var Attendance in attendaces)
            {
                if(Attendance.AttendanceState==AttendanceState.Excused)
                ConsecutiveAbsences++;
               
            }
            return ConsecutiveAbsences;



        }

        public async Task<Dictionary<Guid, double>> GetStudentAttendancePercentageByCourseAsync(Guid courseId)
        {
            var attendaces = await _context.Attendances.Where(a => a.courseId == courseId).ToListAsync();
            var result = new Dictionary<Guid, double>();

            foreach (var studentgroup in attendaces.GroupBy(a => a.courseId))
            {
                var studentid = studentgroup.Key;
                var total = studentgroup.Count();
                var present = studentgroup.Count(a => a.AttendanceState == AttendanceState.Present);
                var percentage = total == 0 ? 0 : (double)present / total * 100;
                result.Add(studentid, Math.Round(percentage,2));
            }

            return result;
        }
    }
}
