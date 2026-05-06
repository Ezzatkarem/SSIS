using SSIS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSIS.Domain.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IReadOnlyList<Course>> GetByDoctorAsync(Guid doctorId);
        Task<Course?> GetByCodeAsync(string code);
        Task<IReadOnlyList<Course>> GetActiveCoursesAsync();
        Task<IReadOnlyList<Course>> GetBySemesterAsync(int semester, int academicYear);
        Task<bool> CodeExistsAsync(string code);
    }
}
