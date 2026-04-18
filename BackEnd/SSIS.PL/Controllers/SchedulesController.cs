using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs.Schedules;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        #region GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _scheduleService.GetAllAsync();
            return Ok(schedules);
        }
        #endregion

        #region GetById
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null)
                return NotFound(new { message = "Schedule not found" });

            return Ok(schedule);
        }
        #endregion

        #region GetByCourse
        [HttpGet("course/{courseId:guid}")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var schedules = await _scheduleService.GetByCourseAsync(courseId);
            return Ok(schedules);
        }
        #endregion

        #region Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var schedule = await _scheduleService.CreateAsync(dto);
            if (schedule == null)
                return BadRequest(new { message = "Failed to create schedule" });

            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
        }
        #endregion

        #region Update
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var schedule = await _scheduleService.UpdateAsync(id, dto);
            if (schedule == null)
                return NotFound(new { message = "Schedule not found" });

            return Ok(schedule);
        }
        #endregion

        #region Delete
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _scheduleService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Schedule not found" });

            return NoContent();
        } 
        #endregion
    }
}
