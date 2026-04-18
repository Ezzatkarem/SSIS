using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs.Grades;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> EnterGrade([FromBody] GradeDTO gradeDTO)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var doctorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var res = await _gradeService.EnterGradeAsync(gradeDTO, doctorId);
            return Ok(res);
        }
        [HttpPut]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateGrade([FromBody] UpdateGradeDTO gradeDTO, Guid gradeId)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var doctorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var res = await _gradeService.UpdateGradesAsync(doctorId, gradeDTO, gradeId);
            return Ok(res);
        }
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentGrades(Guid studentId, [FromQuery] int semester, [FromQuery] int academicYear)
        {
            var res = await _gradeService.GetStudentGradesAsync(studentId, semester, academicYear);
            return Ok(res);

        }
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetGradesByCourse(Guid courseId)
        {
            var res = await _gradeService.GetGradesByCourseAsync(courseId);
            return Ok(res);
        }
        [HttpDelete("{gradeId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteGrade(Guid gradeId)
        {
            var doctorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var res = await _gradeService.deleteGradeAsync(gradeId, doctorId);
            return Ok(res);
        }
            [HttpGet("gpa/{studentId}")]
                    public async Task<IActionResult> GetGpa(Guid studentId, [FromQuery] int? semester = null, [FromQuery] int? academicYear = null)
        {
            var res = await _gradeService.CalculateGpaAsync(studentId, semester, academicYear);
            return Ok(res);
        }
    }
}