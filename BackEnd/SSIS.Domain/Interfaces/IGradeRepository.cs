using SSIS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Interfaces
{
    public interface IGradeRepository: IRepository<Grade>
    {


        Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId, int semster, int academicYear);
        Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId);

        Task<IEnumerable<Grade>> GetByCourseIdAsync(Guid courseId);
        Task<IEnumerable<Grade>> GetByStudentIdAndCourseIdAsync(Guid studentId, Guid courseId, int semester);

        Task<decimal> GetAverageGradeByCourseIdAsync(Guid courseId);
        Task<List<Guid>> GetPassedCourseIdsByStudentAsync(Guid studentId, decimal passingScore);

    }
}
