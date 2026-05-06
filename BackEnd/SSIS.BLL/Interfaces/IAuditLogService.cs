using SSIS.BLL.DTOs.Admin;
using SSIS.BLL.Responce;

namespace SSIS.BLL.Interfaces
{
    public interface IAuditLogService
    {
        Task<Responce<List<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? action = null, string? entity = null);
        Task<Responce<AuditLogDto>> GetAuditLogByIdAsync(Guid id);
        Task<Responce<List<AuditLogDto>>> GetUserActivityAsync(Guid userId);
        Task LogActionAsync(Guid? userId, string action, string entity, Guid? entityId, string? oldValues, string? newValues, string? ipAddress, string? userAgent);
    }
}