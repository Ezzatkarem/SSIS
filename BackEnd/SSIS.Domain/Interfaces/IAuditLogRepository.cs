using SSIS.Domain.Entities;

namespace SSIS.Domain.Interfaces
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(Guid userId);
        Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entity, Guid? entityId = null);
        Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<IReadOnlyList<AuditLog>> GetByActionAsync(string action);
    }
}