using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSIS.DAL.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Enrollment>> GetByStudentAsync(Guid studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.IsActive)
                .Include(e => e.Course)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Enrollment>> GetByCourseAsync(Guid courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.IsActive)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Enrollment>> GetActiveEnrollmentsAsync()
        {
            return await _context.Enrollments
                .Where(e => e.IsActive)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid studentId, Guid courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.IsActive);
        }

        public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }
    }
}
