using SSIS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSIS.Domain.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IReadOnlyList<Enrollment>> GetByStudentAsync(Guid studentId);
        Task<IReadOnlyList<Enrollment>> GetByCourseAsync(Guid courseId);
        Task<IReadOnlyList<Enrollment>> GetActiveEnrollmentsAsync();
        Task<bool> ExistsAsync(Guid studentId, Guid courseId);
        Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
    }
}
