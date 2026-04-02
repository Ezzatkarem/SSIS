using SSIS.BLL.DTOs.Schedules;

namespace SSIS.BLL.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleDto?> CreateAsync(CreateScheduleDto dto);
        Task<ScheduleDto?> UpdateAsync(Guid id, UpdateScheduleDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ScheduleDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<ScheduleDto>> GetByCourseAsync(Guid courseId);
        Task<IReadOnlyList<ScheduleDto>> GetAllAsync();
    }
}
