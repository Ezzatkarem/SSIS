using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using SSIS.BLL.DTOs.Courses;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.BLL.Services.Implementation
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepo _userRepository;

        public CourseService(IUnitOfWork unitOfWork, IUserRepo userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        #region CreateAsync
        public async Task<CourseDto?> CreateAsync(CreateCourseDto dto)
        {
            // Check if code already exists
            if (await _unitOfWork.Courses.CodeExistsAsync(dto.Code))
                return null;

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Credits = dto.Credits,
                Description = dto.Description,
                Semester = dto.Semester,
                AcademicYear = dto.AcademicYear,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(course);
        }

        #endregion
        #region UpdateAsync
        public async Task<CourseDto?> UpdateAsync(Guid id, UpdateCourseDto dto)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null || course.IsDeleted)
                return null;

            course.Name = dto.Name;
            course.Credits = dto.Credits;
            course.Description = dto.Description;
            course.IsActive = dto.IsActive;
            course.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(course);
        }
        #endregion

        #region DeleteAsync
        public async Task<bool> DeleteAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null || course.IsDeleted)
                return false;

            // Check if course has enrollments
            var enrollments = await _unitOfWork.Enrollments.GetByCourseAsync(id);
            if (enrollments.Any(e => e.IsActive))
                return false; // Cannot delete course with active enrollments

            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion
        #region GetByIdAsync
        public async Task<CourseDto?> GetByIdAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null || course.IsDeleted)
                return null;

            return MapToDto(course);
        }
        #endregion

        #region GetAllAsync
        public async Task<IReadOnlyList<CourseDto>> GetAllAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();
            return courses.Where(c => !c.IsDeleted).Select(MapToDto).ToList();
        }
        #endregion

        #region GetByDoctorAsync
        public async Task<IReadOnlyList<CourseDto>> GetByDoctorAsync(Guid doctorId)
        {
            var courses = await _unitOfWork.Courses.GetByDoctorAsync(doctorId);
            return courses.Select(MapToDto).ToList();
        }
        #endregion

        #region AssignDoctorAsync
        public async Task<bool> AssignDoctorAsync(Guid courseId, Guid doctorId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
                return false;

            // Verify doctor exists and has Doctor role
            var doctor = await _userRepository.GetByIdAsync(doctorId);
            if (doctor == null || doctor.IsDeleted || doctor.Role != UserRole.Doctor)
                return false;

            course.DoctorId = doctorId;
            course.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        #endregion

        #region GetActiveCoursesAsync
        public async Task<IReadOnlyList<CourseDto>> GetActiveCoursesAsync()
        {
            var courses = await _unitOfWork.Courses.GetActiveCoursesAsync();
            return courses.Select(MapToDto).ToList();
        } 
        #endregion

        private CourseDto MapToDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Code = course.Code,
                Credits = course.Credits,
                Description = course.Description,
                DoctorId = course.DoctorId,
                DoctorName = course.Doctor?.FullName,
                Semester = course.Semester,
                AcademicYear = course.AcademicYear,
                IsActive = course.IsActive,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };
        }
    }
}
