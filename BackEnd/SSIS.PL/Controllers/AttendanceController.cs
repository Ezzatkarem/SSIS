using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paymob.Net.Models;
using SSIS.BLL.DTOs.Attendances;
using SSIS.BLL.Services.Interfaces;
using System.Security.Claims;

namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;
        private readonly IUserService userService;
        private readonly INotificationService notificationService;

        public AttendanceController(IAttendanceService attendanceService, IUserService userService, INotificationService notificationService)
        {
            this.attendanceService = attendanceService;
            this.userService = userService;
            this.notificationService = notificationService;
        }
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
        [HttpGet("student/{studentid}")]
        public async Task<IActionResult> GetstudentAttendance(Guid studentid)

        {
            var currentuser = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var currentrole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (currentrole != "Admin" && currentrole != "Doctor" && currentuser != studentid)
                return Forbid();
            var attendance = await attendanceService.GetStudentAttendanceAsync(studentid);
            if (!attendance.IsSuccess)
            {
                return BadRequest(new { message = attendance.Message });
            }
            return Ok(attendance);
        }
        [HttpGet("course/{courseid}")]
        [Authorize(Roles ="Admin,Doctor")]
        
        public async Task<IActionResult> GetcourseAttendance(Guid courseid)

        {
            var attendance = await attendanceService.GetcourseAttendanceAsync(courseid);
            if (!attendance.IsSuccess)
            {
                return BadRequest(new { message = attendance.Message });
            }
            return Ok(attendance);
        }

        [HttpGet("student/{studentid}/persentage")]

        public async Task<IActionResult> GetstudentAttendancePersentageAsync(Guid studentid)

        {
            var currentuser = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var currentrole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (currentrole != "Admin" && currentrole != "Doctor" && currentuser != studentid)
                return Forbid();
            var attendance = await attendanceService.GetStudentAttendancePersentageAsync(studentid);
            if (!attendance.IsSuccess)
            {
                return BadRequest(new { message = attendance.Message });
            }
            return Ok(attendance);
        }

        [HttpGet("course/{courseid}/student-persentage")]
        [Authorize(Roles = "Admin,Doctor")]

        public async Task<IActionResult> GetcourseAttendancePersentageAsync(Guid courseid)

        {

            var attendance = await attendanceService.GetCourseAttendancePersentageAsync(courseid);
            if (!attendance.IsSuccess)
            {
                return BadRequest(new { message = attendance.Message });
            }
            return Ok(attendance);
        }


        [HttpGet("course/{courseid}/overall-persentage")]
        [Authorize(Roles = "Admin,Doctor")]

        public async Task<IActionResult> GetOverAllcourseAttendancePersentageAsync(Guid courseid)

        {

            var attendance = await attendanceService.GetOverAllCourseAttendancePersentageAsync(courseid);
            if (!attendance.IsSuccess)
            {
                return BadRequest(new { message = attendance.Message });
            }
            return Ok(attendance);
        }
        [HttpGet("student/{studentid}/overall-persentage")]

        public async Task<IActionResult> GetOverAllstudentAttendancePersentageAsync(Guid studentid)

        {
            var currentuser = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var currentrole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (currentrole != "Admin" && currentrole != "Doctor" && currentuser != studentid)
                return Forbid();

            var attendance = await attendanceService.GetOverAllStudentAttendancePersentageAsync(studentid);
            if (!attendance.IsSuccess)
            {
                return BadRequest(new { message = attendance.Message });
            }
            return Ok(attendance);
        }
        [HttpPost("take")]
        [Authorize(Roles ="Admin,Doctor")]
        public async Task<IActionResult> TakeAttendanceAsync( [FromBody]TakeAttendanceDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var doctorid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var res = await attendanceService.TakeAttendanceAsync(dto, doctorid);
            if(!res.IsSuccess)
            {
                return BadRequest(new {message=res.Message});
            }
            return Ok(new {message=res.Message,res.Data});

        }
        [HttpGet("student/{studentid}/course/{courseid}/consecutive-Absences")]
        public async Task<IActionResult> GetconsecutiveAbsences(Guid studentid,Guid courseid)
        {
            var currentuser = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var currentrole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            if (currentrole != "Admin" && currentrole != "Doctor" && currentuser != studentid)
                return Forbid();
            var result = await attendanceService.GetConsecutiveAbsentAttendanceAsync(courseid, studentid);
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message, result.Data });
        }

    }
}
