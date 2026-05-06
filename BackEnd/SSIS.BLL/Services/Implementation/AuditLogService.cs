using AutoMapper;
using SSIS.BLL.DTOs.Admin;
using SSIS.BLL.Interfaces;
using SSIS.BLL.Responce;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;

namespace SSIS.BLL.Services.Implementation
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IMapper _mapper;

        public AuditLogService(IAuditLogRepository auditLogRepository, IMapper mapper)
        {
            _auditLogRepository = auditLogRepository;
            _mapper = mapper;
        }

        public async Task<Responce<List<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? action = null, string? entity = null)
        {
            IEnumerable<AuditLog> logs;

            if (!string.IsNullOrEmpty(action))
                logs = await _auditLogRepository.GetByActionAsync(action);
            else if (!string.IsNullOrEmpty(entity))
                logs = await _auditLogRepository.GetByEntityAsync(entity);
            else
                logs = await _auditLogRepository.GetAllAsync();

            var pagedLogs = logs
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<List<AuditLogDto>>(pagedLogs);
            return new Responce<List<AuditLogDto>>(dtos, true, null!);
        }

        public async Task<Responce<AuditLogDto>> GetAuditLogByIdAsync(Guid id)
        {
            var log = await _auditLogRepository.GetByIdAsync(id);
            if (log == null)
                return new Responce<AuditLogDto>(null!, false, "Audit log not found");

            var dto = _mapper.Map<AuditLogDto>(log);
            return new Responce<AuditLogDto>(dto, true, null!);
        }

        public async Task<Responce<List<AuditLogDto>>> GetUserActivityAsync(Guid userId)
        {
            var logs = await _auditLogRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<List<AuditLogDto>>(logs);
            return new Responce<List<AuditLogDto>>(dtos, true, null!);
        }

        public async Task LogActionAsync(Guid? userId, string action, string entity, Guid? entityId, string? oldValues, string? newValues, string? ipAddress, string? userAgent)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);
        }
    }
}