using SSIS.BLL.DTOs.Courses;
using SSIS.BLL.Services.Interfaces;
using SSIS.DAL.Repositories;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Services.Implementation
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepo _userRepository;
        private readonly ICoursePrerequisiteRepository coursePrerequisiteRepository;
        private readonly IGradeRepository gradeRepository ;

        public CourseService(IUnitOfWork unitOfWork, IUserRepo userRepository, ICoursePrerequisiteRepository coursePrerequisiteRepository, IGradeRepository gradeRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            this.coursePrerequisiteRepository = coursePrerequisiteRepository;
            this.gradeRepository = gradeRepository;
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


        #region CreateWithPrereqAsync
        public async Task<CourseDto?> CreateWithPrereqAsync(CreateCourseWithPrereqDto dto)
        {
            if (await _unitOfWork.Courses.CodeExistsAsync(dto.Code))
                return null;

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Credits = dto.Credits,
                Description = dto.Descreption,
                Semester = dto.semester,
                AcademicYear = dto.AcedemicYear,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.DoctorID.HasValue)
            {
                var doctor = await _userRepository.GetByIdAsync(dto.DoctorID.Value);
                if (doctor != null && doctor.Role == UserRole.Doctor)
                    course.DoctorId = dto.DoctorID;
            }

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            foreach (var prereqId in dto.PrerequistisIds)
            {
                var prereqCourse = await _unitOfWork.Courses.GetByIdAsync(prereqId);
                if (prereqCourse == null || prereqCourse.IsDeleted)
                    continue;

                var prereq = new CoursePrerequesite
                {
                    Id = Guid.NewGuid(),
                    Courseid = course.Id,
                    PrerequesiteCourseid = prereqId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.coursePrerequisite.AddAsync(prereq);
            }

            await _unitOfWork.SaveChangesAsync();
            return MapToDto(course);
        }
        #endregion

        #region GetAvailableCoursesForStudentAsync
        public async Task<List<CourseDto>> GetAvailableCoursesForStudentAsync(Guid studentId)
        {
            var passedCourses = await gradeRepository.GetPassedCourseIdsByStudentAsync(studentId, 60);
            var enrolledCourses = await _unitOfWork.Enrollments.GetCourseIdsByStudentAsync(studentId);
            var allCourses = await _unitOfWork.Courses.GetAllAsync();
            allCourses = allCourses.Where(c => !c.IsDeleted).ToList();

            var availableCourses = new List<CourseDto>();

            foreach (var course in allCourses)
            {
                if (enrolledCourses.Contains(course.Id) || passedCourses.Contains(course.Id))
                    continue;

                var prerequisites = await _unitOfWork.coursePrerequisite
                    .GetPrerequisiteIdsByCourseAsync(course.Id);

                if (!prerequisites.Any())
                {
                    availableCourses.Add(MapToDto(course));
                    continue;
                }

                var hasAllPrerequisites = prerequisites.All(p => passedCourses.Contains(p));
                if (hasAllPrerequisites)
                {
                    availableCourses.Add(MapToDto(course));
                }
            }

            return availableCourses;
        }
        #endregion

        #region SelfEnrollAsync
        public async Task<(bool Success, string Message)> SelfEnrollAsync(Guid studentId, Guid courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null || course.IsDeleted)
                return (false, "Course not found");

            var isEnrolled = await _unitOfWork.Enrollments.ExistsAsync(studentId, courseId);
            if (isEnrolled)
                return (false, "Already enrolled in this course");

            var passedCourses = await gradeRepository.GetPassedCourseIdsByStudentAsync(studentId, 60);
            var prerequisites = await _unitOfWork.coursePrerequisite.GetPrerequisiteIdsByCourseAsync(courseId);

            if (prerequisites.Any())
            {
                var missingPrereqs = prerequisites.Where(p => !passedCourses.Contains(p)).ToList();
                if (missingPrereqs.Any())
                {
                    var missingNames = await _unitOfWork.coursePrerequisite.GetNamesByIdsAsync(missingPrereqs);
                    return (false, $"Missing prerequisites: {string.Join(", ", missingNames)}");
                }
            }

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                IsActive = true,
                Semester = course.Semester,
                AcademicYear = course.AcademicYear,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return (true, $"Successfully enrolled in {course.Name}");
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
