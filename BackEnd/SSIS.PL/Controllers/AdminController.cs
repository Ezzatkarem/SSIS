using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AdminController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? action = null, [FromQuery] string? entity = null)
        {
            var result = await _auditLogService.GetAuditLogsAsync(page, pageSize, action, entity);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("audit-logs/{id}")]
        public async Task<IActionResult> GetAuditLogById(Guid id)
        {
            var result = await _auditLogService.GetAuditLogByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("audit-logs/user/{userId}")]
        public async Task<IActionResult> GetUserActivity(Guid userId)
        {
            var result = await _auditLogService.GetUserActivityAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            return Ok(result);
        }
    }
}