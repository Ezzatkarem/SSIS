using SSIS.BLL.DTOs.Courses;

namespace SSIS.BLL.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto?> CreateAsync(CreateCourseDto dto);
        Task<CourseDto?> UpdateAsync(Guid id, UpdateCourseDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<CourseDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<CourseDto>> GetAllAsync();
        Task<IReadOnlyList<CourseDto>> GetByDoctorAsync(Guid doctorId);
        Task<bool> AssignDoctorAsync(Guid courseId, Guid doctorId);
        Task<IReadOnlyList<CourseDto>> GetActiveCoursesAsync();
    }
}
