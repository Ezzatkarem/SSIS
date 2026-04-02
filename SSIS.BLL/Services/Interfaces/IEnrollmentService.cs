using SSIS.BLL.DTOs.Enrollments;

namespace SSIS.BLL.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto?> EnrollAsync(CreateEnrollmentDto dto);
        Task<bool> UnenrollAsync(Guid enrollmentId);
        Task<StudentCoursesDto?> GetStudentCoursesAsync(Guid studentId);
        Task<CourseStudentsDto?> GetCourseStudentsAsync(Guid courseId);
        Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId);
        Task<EnrollmentDto?> GetByIdAsync(Guid id);
    }
}
