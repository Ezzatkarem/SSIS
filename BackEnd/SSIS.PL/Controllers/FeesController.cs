using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs.Fee;
using SSIS.BLL.Services.Interfaces;

namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeesController : ControllerBase
    {
        private readonly IFeeService feeService;

        public FeesController(IFeeService feeService)
        {
            this.feeService = feeService;
        }
        #region CreateFee
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFee([FromBody] CreateFeeDto dto)
        {
            try
            {
                var res = feeService.CreateFeeForStudentAsync(dto);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        #endregion

        #region AutoGenerateFees
        [HttpPost("auto-generate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AutoGenerateFees([FromBody] FeeSettingsDto dto)
        {
            try
            {
                var res = feeService.AutoGenerateFeesAsync(dto);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        #endregion

        #region UpdateFees
        [HttpPut("{feeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFees(Guid feeId, [FromBody] UpdateFee dto)
        {
            try
            {
                var res = feeService.UpdateFeeAsync(feeId, dto);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        #endregion

        #region DeleteFee
        [HttpDelete("{feeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFee(Guid feeId)
        {
            try
            {
                var res = feeService.DeleteFeeAsync(feeId);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        #endregion

        #region GetAllFees
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllFees()
        {
            try
            {
                var res = feeService.GetAllFeesAsync();
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region GetFeeById
        [HttpGet("{feeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFeeById(Guid feeId)
        {
            try
            {
                var res = feeService.GetFeeByIdAsync(feeId);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region GetFeesByStudentId
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetFeesByStudentId(Guid studentId)
        {
            try
            {
                var res = feeService.GetFeesByStudentAsync(studentId);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Getmyfees
        [HttpGet("my-fees")]
        public async Task<IActionResult> Getmyfees()
        {
            try
            {
                var studentid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
                var res = feeService.GetMyFeesAsync(studentid);
                return Ok(res.Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        } 
        #endregion


    }
}
