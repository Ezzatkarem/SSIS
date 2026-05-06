using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paymob.Net.Models;
using SSIS.BLL.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("admin-dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var Adminid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value!);
            var d = await _reportService.GetAdminDashboardAsync(Adminid);
            return Ok(d);

        }
        [HttpGet("doctor-dashboard")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetdoctorDashboard()
        {
            var doctorid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value!);
            var d = await _reportService.GetDoctorDashboardAsync(doctorid);
            return Ok(d);

        }
        [HttpGet("student-dashboard")]
        public async Task<IActionResult> GetstudentDashboard()
        {
            var studentid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value!);
            var d = await _reportService.GetStudentDashboardAsync(studentid);
            return Ok(d);

        }

        [HttpGet("top-students")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTopStudents([FromQuery] int count = 10)
        {
            var topStudents = await _reportService.GetTopStudentsAsync(count);
            return Ok(topStudents);
        }

        [HttpGet("course-statistics/{courseId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetCourseStatistics(Guid courseId)
        {
            var stats = await _reportService.GetCourseStatisticsAsync(courseId);
            return Ok(stats);
        }
    }
}
