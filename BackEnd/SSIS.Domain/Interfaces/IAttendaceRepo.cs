using SSIS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Interfaces
{
    public interface IAttendaceRepo :IRepository<Attendance>
    {
        Task<IReadOnlyList<Attendance>> GetByStudentIdAsync(Guid studentId);
        Task<IReadOnlyList<Attendance>> GetByCourseIdAsync(Guid courseid);
        Task<Attendance?> GetByStudentCourseAndDateAsync(Guid studentId, Guid courseId, DateOnly date);
        Task<Dictionary<Guid, double>> GetAttendancePercentageByCourseAsync(Guid studentId);
        Task<Dictionary<Guid, double>> GetStudentAttendancePercentageByCourseAsync(Guid courseId);
        Task< double> GetOverAllAttendancePercentageForStudentAsync(Guid StudentId);
        Task<double> GetOveerAllAttendancePercentageForCourseAsync(Guid courseId);
        Task<int> GetConsecutiveAbsencesAsync(Guid studentId,Guid courseId);


    }

}
