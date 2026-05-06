using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;


namespace SSIS.DAL.Repositories
{
    public class GradeRepository : Repository<Grade>, IGradeRepository
    {
        private readonly AppDbContext _context;
        public GradeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<decimal> GetAverageGradeByCourseIdAsync(Guid courseId)
        {
           return await _context.Grades.Where(p=>p.CourseId== courseId).AverageAsync(g => g.Score);
        }

        public async Task<IEnumerable<Grade>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Grades
             .Where(g => g.CourseId == courseId)
             .Include(g => g.Student)
             .ToListAsync();
        }
        public async Task<List<Guid>> GetPassedCourseIdsByStudentAsync(Guid studentId, decimal passingScore)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId && g.Score >= passingScore)
                .Select(g => g.CourseId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Grade>> GetByStudentIdAndCourseIdAsync(Guid studentId, Guid courseId, int semester)
        {
            return await _context.Grades.Where( p=>p.StudentId == studentId && p.CourseId == courseId && p.Semester == semester).ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId, int semester, int academicYear)
        {
            return await _context.Grades
            .Where(g => g.StudentId == studentId&&g.Semester==semester&&g.AcademicYear==academicYear)
            .Include(g => g.Course)
            .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.Grades
           .Where(g => g.StudentId == studentId )
           .Include(g => g.Course)
           .ToListAsync();
        }
    }
}
