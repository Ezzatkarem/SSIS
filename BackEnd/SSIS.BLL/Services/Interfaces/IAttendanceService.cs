using SSIS.BLL.DTOs.Attendances;
using SSIS.BLL.Responce;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<Responce<AttendanceDto>> GetStudentAttendanceAsync(Guid studentid);
        Task<Responce<AttendanceDto>> GetcourseAttendanceAsync(Guid Courseid);

        Task<Responce<List<AttendancePercentageDto>>> GetStudentAttendancePersentageAsync(Guid studentid);
        Task<Responce<List<StudentAttendancePercentageDto>>> GetCourseAttendancePersentageAsync(Guid studentid);
        Task<Responce<double>> GetOverAllStudentAttendancePersentageAsync(Guid StudentId);
        Task<Responce<double>> GetOverAllCourseAttendancePersentageAsync(Guid courseid);



        Task<Responce<object>> TakeAttendanceAsync(TakeAttendanceDto dto, Guid doctorid);
        Task<Responce<int>> GetConsecutiveAbsentAttendanceAsync(Guid CourseId, Guid StudentId);

    }
}
