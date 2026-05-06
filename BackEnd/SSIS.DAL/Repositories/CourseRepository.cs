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
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Course>> GetByDoctorAsync(Guid doctorId)
        {
            return await _context.Courses
                .Where(c => c.DoctorId == doctorId && !c.IsDeleted)
                .Include(c => c.Doctor)
                .ToListAsync();
        }

        public async Task<Course?> GetByCodeAsync(string code)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted);
        }

        public async Task<IReadOnlyList<Course>> GetActiveCoursesAsync()
        {
            return await _context.Courses
                .Where(c => c.IsActive && !c.IsDeleted)
                .Include(c => c.Doctor)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Course>> GetBySemesterAsync(int semester, int academicYear)
        {
            return await _context.Courses
                .Where(c => c.Semester == semester && c.AcademicYear == academicYear && !c.IsDeleted)
                .Include(c => c.Doctor)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Courses
                .AnyAsync(c => c.Code == code && !c.IsDeleted);
        }
    }
}
