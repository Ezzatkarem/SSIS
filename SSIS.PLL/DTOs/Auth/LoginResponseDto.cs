

namespace SSIS.PLL.DTOs.Login
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

    }
}
