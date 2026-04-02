using Microsoft.EntityFrameworkCore;
using SSIS.BLL.Services.Interfaces;
using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.BLL.DTOs.Schedules;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.BLL.Services.Implementation
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleDto?> CreateAsync(CreateScheduleDto dto)
        {
            // Verify course exists
            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null || course.IsDeleted)
                return null;

            // Validate time range
            if (dto.EndTime <= dto.StartTime)
                return null;

            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                CourseId = dto.CourseId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Room = dto.Room,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            return MapToDto(schedule);
        }

        public async Task<ScheduleDto?> UpdateAsync(Guid id, UpdateScheduleDto dto)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null || schedule.IsDeleted)
                return null;

            // Validate time range
            if (dto.EndTime <= dto.StartTime)
                return null;

            schedule.DayOfWeek = dto.DayOfWeek;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.Room = dto.Room;
            schedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(schedule);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null || schedule.IsDeleted)
                return false;

            schedule.IsDeleted = true;
            schedule.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ScheduleDto?> GetByIdAsync(Guid id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (schedule == null)
                return null;

            return MapToDto(schedule);
        }

        public async Task<IReadOnlyList<ScheduleDto>> GetByCourseAsync(Guid courseId)
        {
            var schedules = await _context.Schedules
                .Include(s => s.Course)
                .Where(s => s.CourseId == courseId && !s.IsDeleted)
                .ToListAsync();

            return schedules.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<ScheduleDto>> GetAllAsync()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Course)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            return schedules.Select(MapToDto).ToList();
        }

        private ScheduleDto MapToDto(Schedule schedule)
        {
            return new ScheduleDto
            {
                Id = schedule.Id,
                CourseId = schedule.CourseId,
                CourseName = schedule.Course?.Name ?? string.Empty,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room
            };
        }
    }
}
