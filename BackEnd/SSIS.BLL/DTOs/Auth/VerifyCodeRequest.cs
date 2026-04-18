using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.PLL.DTOs.Auth
{
    public class VerifyCodeRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
