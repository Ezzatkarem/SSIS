using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SSIS.PLL.DTOs.Auth;

using SSIS.PLL.Services.Interfaces;
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
        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (data, errors) = await _userService.RegisterAsync(request);

            if (errors.Length > 0)
                return BadRequest(new { errors });

            return Ok(data);
        }
        #endregion


        #region Login
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
        #endregion

        #region SendVerificationCode
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] string email)
        {
            var (seccess, message) = await _userService.SendEmailVerificationCodeAsync(email);
            if (!seccess) return BadRequest(new { message });
            return Ok(new { message });

        } 
        #endregion

        #region VerifyEmail
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyCodeRequest Request)
        {
            var (seccess, message) = await _userService.VerifyEmailCodeAsync(Request.Email, Request.Code);
            if (!seccess) return BadRequest(new { message });
            return Ok(new { message });
        }
        #endregion

        #region ResendCode
        [HttpPost("resend-code")]
        public async Task<IActionResult> ResendCode([FromBody] string email)
        {
            var (seccess, message) = await _userService.ResendEmailVerificationCodeAsync(email);
            if (!seccess) return BadRequest(new { message });
            return Ok(new { message });
        } 
        #endregion
    }
}
