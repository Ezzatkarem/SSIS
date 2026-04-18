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
    public class ScheduleRepository : Repository<Schedule>, IRepository<Schedule>
    {
        public ScheduleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Schedule>> GetByCourseAsync(Guid courseId)
        {
            return await _context.Schedules
                .Where(s => s.CourseId == courseId && !s.IsDeleted)
                .Include(s => s.Course)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Schedule>> GetActiveSchedulesAsync()
        {
            return await _context.Schedules
                .Where(s => !s.IsDeleted)
                .Include(s => s.Course)
                .ToListAsync();
        }
    }
}
