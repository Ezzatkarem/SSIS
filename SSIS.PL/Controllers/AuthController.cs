using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SSIS.BLL.DTOs.Login;
using SSIS.BLL.Interfaces;
using SSIS.BLL.Services.Interfaces;
namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthController(IUserService userService, IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
        }
        // POST: api/v1/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterAsync(request);

            if (result == null)
                return BadRequest(new { message = "Email already exists" });

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }


    }
}
