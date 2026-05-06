using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;

namespace SSIS.DAL.Repositories
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entity, Guid? entityId = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.Entity == entity);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            return await query.OrderByDescending(a => a.Timestamp).ToListAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.Timestamp >= from && a.Timestamp <= to)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetByActionAsync(string action)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.Action == action)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}