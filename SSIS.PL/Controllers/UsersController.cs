using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SSIS.PLL.Services.Interfaces;
using SSIS.BLL.DTOs.Users;
using SSIS.BLL.Interfaces;
using SSIS.BLL.Services.Interfaces; 

namespace SSIS.PL.Controllers.v1    
{
    [Route("api/v1/[controller]")]  
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        #region GetAll
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }
        #endregion

        #region GetById
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }
            return Ok(user);
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [Authorize (Roles ="Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.UpdateAsync(id, request);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }
            return Ok(user);
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);  // ✅ bool
            if (!result)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }
            return Ok(new { message = "User deleted successfully" });
        }
        #endregion
        #region ChangePassword

        [HttpPost("{id}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePasswordAsync(id, request);  // ✅ bool
            if (!result)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }
            return Ok(new { message = "Password changed successfully" });
        } 
        #endregion
    }
}