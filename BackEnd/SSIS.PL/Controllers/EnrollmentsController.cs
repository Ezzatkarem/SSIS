using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs.Enrollments;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly INotificationService notificationService ;

        public EnrollmentsController(IEnrollmentService enrollmentService, INotificationService notificationService)
        {
            _enrollmentService = enrollmentService;
            this.notificationService = notificationService;
        }

        #region Enroll
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var enrollment = await _enrollmentService.EnrollAsync(dto);
            if (enrollment == null)
                return BadRequest(new { message = "Failed to enroll student" });

            await notificationService.NotifyEnrollmentCreatedAsync(enrollment.StudentId, enrollment.CourseName);
            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
        }
        #endregion

        #region Unenroll
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unenroll(Guid  id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);

            var result = await _enrollmentService.UnenrollAsync(id);
            if (!result)
                return NotFound(new { message = "Enrollment not found" });

            await notificationService.NotifyEnrollmentRemovedAsync(enrollment.StudentId, enrollment.CourseName);
            return NoContent();
        }
        #endregion

        #region GetById
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            return Ok(enrollment);
        }
        #endregion

        #region GetStudentCourses
        [HttpGet("student/{studentId:guid}")]
        public async Task<IActionResult> GetStudentCourses(Guid studentId)
        {
            var result = await _enrollmentService.GetStudentCoursesAsync(studentId);
            if (result == null)
                return NotFound(new { message = "Student not found" });

            return Ok(result);
        }
        #endregion

        #region GetCourseStudents
        [HttpGet("course/{courseId:guid}")]
        public async Task<IActionResult> GetCourseStudents(Guid courseId)
        {
            var result = await _enrollmentService.GetCourseStudentsAsync(courseId);
            if (result == null)
                return NotFound(new { message = "Course not found" });

            return Ok(result);
        }
        #endregion

        #region CheckEnrollment
        [HttpGet("check")]
        public async Task<IActionResult> CheckEnrollment([FromQuery] Guid studentId, [FromQuery] Guid courseId)
        {
            var isEnrolled = await _enrollmentService.CheckEnrollmentAsync(studentId, courseId);
            return Ok(new { isEnrolled });
        } 
        #endregion
    }
}
