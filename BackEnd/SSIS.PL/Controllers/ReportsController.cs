using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.Interfaces;
using System.Security.Claims;

namespace SSIS.PL.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("course/{courseId}/statistics")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetCourseGradeStatistics(Guid courseId)
        {
            var result = await _reportService.GetCourseGradeStatisticsAsync(courseId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("at-risk")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAtRiskStudents()
        {
            var result = await _reportService.GetAtRiskStudentsAsync();
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("student/{studentId}/analytics")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetStudentAnalytics(Guid studentId)
        {
            var result = await _reportService.GetStudentAnalyticsAsync(studentId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }
    }
}