using Microsoft.EntityFrameworkCore;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using SSIS.DAL.Data;

namespace SSIS.DAL.Repositories
{
    public class CoursePrerequisiteRepository : Repository<CoursePrerequesite>, ICoursePrerequisiteRepository
    {
        public CoursePrerequisiteRepository(AppDbContext context) : base(context) { }

        public async Task<List<Guid>> GetPrerequisiteIdsByCourseAsync(Guid courseId)
        {
            return await _context.coursePrerequesites
                .Where(cp => cp.Courseid == courseId)
                .Select(cp => cp.PrerequesiteCourseid)
                .ToListAsync();
        }

        public async Task<List<string>> GetNamesByIdsAsync(List<Guid> courseIds)
        {
            return await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .Select(c => c.Name)
                .ToListAsync();
        }
    }
}