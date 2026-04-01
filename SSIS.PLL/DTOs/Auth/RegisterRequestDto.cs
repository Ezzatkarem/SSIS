using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace SSIS.PLL.DTOs.Login
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public IFormFile? DocumentsFile { get; set; }

    }
}
