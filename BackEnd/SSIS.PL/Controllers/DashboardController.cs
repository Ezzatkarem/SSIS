using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.Interfaces;
using System.Security.Claims;

namespace SSIS.PL.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IReportService _reportService;

        public DashboardController(IReportService reportService)
        {
            _reportService = reportService;
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

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _reportService.GetAdminDashboardAsync();
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorDashboard()
        {
            var doctorId = GetCurrentUserId();
            var result = await _reportService.GetDoctorDashboardAsync(doctorId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentDashboard()
        {
            var studentId = GetCurrentUserId();
            var result = await _reportService.GetStudentDashboardAsync(studentId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetStudentDashboardById(Guid studentId)
        {
            var result = await _reportService.GetStudentDashboardAsync(studentId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }
    }
}