using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using SSIS.BLL.DTOs.Enrollments;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.BLL.Services.Implementation
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepo _userRepository;

        public EnrollmentService(IUnitOfWork unitOfWork, IUserRepo userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        #region EnrollAsync
        public async Task<EnrollmentDto?> EnrollAsync(CreateEnrollmentDto dto)
        {
            // Verify student exists and has Student role
            var student = await _userRepository.GetByIdAsync(dto.StudentId);
            if (student == null || student.IsDeleted || student.Role != UserRole.Student)
                return null;

            // Verify course exists and is active
            var course = await _unitOfWork.Courses.GetByIdAsync(dto.CourseId);
            if (course == null || course.IsDeleted || !course.IsActive)
                return null;

            // Check if already enrolled
            if (await _unitOfWork.Enrollments.ExistsAsync(dto.StudentId, dto.CourseId))
                return null;

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(enrollment);
        }
        #endregion

        #region UnenrollAsync
        public async Task<bool> UnenrollAsync(Guid enrollmentId)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
            if (enrollment == null || enrollment.IsDeleted || !enrollment.IsActive)
                return false;

            enrollment.IsActive = false;
            enrollment.IsDeleted = true;
            enrollment.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.Enrollments.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        #endregion

        #region GetStudentCoursesAsync
        public async Task<StudentCoursesDto?> GetStudentCoursesAsync(Guid studentId)
        {
            var student = await _userRepository.GetByIdAsync(studentId);
            if (student == null || student.IsDeleted)
                return null;

            var enrollments = await _unitOfWork.Enrollments.GetByStudentAsync(studentId);

            return new StudentCoursesDto
            {
                StudentId = studentId,
                StudentName = student.FullName,
                Courses = enrollments.Select(e => new CourseInfoDto
                {
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name,
                    CourseCode = e.Course.Code,
                    Credits = e.Course.Credits,
                    EnrollmentDate = e.EnrollmentDate
                }).ToList()
            };
        }
        #endregion

        #region GetCourseStudentsAsync
        public async Task<CourseStudentsDto?> GetCourseStudentsAsync(Guid courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
                return null;

            var enrollments = await _unitOfWork.Enrollments.GetByCourseAsync(courseId);

            return new CourseStudentsDto
            {
                CourseId = courseId,
                CourseName = course.Name,
                CourseCode = course.Code,
                Students = enrollments.Select(e => new StudentInfoDto
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    Email = e.Student.Email,
                    EnrollmentDate = e.EnrollmentDate
                }).ToList()
            };
        }
        #endregion

        #region CheckEnrollmentAsync
        public async Task<bool> CheckEnrollmentAsync(Guid studentId, Guid courseId)
        {
            return await _unitOfWork.Enrollments.ExistsAsync(studentId, courseId);
        }
        #endregion

        #region GetByIdAsync
        public async Task<EnrollmentDto?> GetByIdAsync(Guid id)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null || enrollment.IsDeleted)
                return null;

            return MapToDto(enrollment);
        }

        #endregion
        private EnrollmentDto MapToDto(Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student?.FullName ?? string.Empty,
                CourseId = enrollment.CourseId,
                CourseName = enrollment.Course?.Name ?? string.Empty,
                CourseCode = enrollment.Course?.Code ?? string.Empty,
                EnrollmentDate = enrollment.EnrollmentDate,
                IsActive = enrollment.IsActive
            };
        }
    }
}
