using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs;
using SSIS.BLL.DTOs.Courses;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllAsync();
            return Ok(courses);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        [HttpGet("doctor/{doctorId:guid}")]
        public async Task<IActionResult> GetByDoctor(Guid doctorId)
        {
            var courses = await _courseService.GetByDoctorAsync(doctorId);
            return Ok(courses);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.CreateAsync(dto);
            if (course == null)
                return BadRequest(new { message = "Course code already exists" });

            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.UpdateAsync(id, dto);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _courseService.DeleteAsync(id);
            if (!result)
                return BadRequest(new { message = "Cannot delete course with active enrollments" });

            return NoContent();
        }

        [HttpPost("{id:guid}/assign-doctor")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDoctor(Guid id, [FromBody] AssignDoctorDto dto)
        {
            var result = await _courseService.AssignDoctorAsync(id, dto.DoctorId);
            if (!result)
                return BadRequest(new { message = "Failed to assign doctor" });

            return Ok(new { message = "Doctor assigned successfully" });
        }
    }
}
