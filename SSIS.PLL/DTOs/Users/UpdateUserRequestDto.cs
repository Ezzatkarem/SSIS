using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.PLL.DTOs.Users
{
    public class UpdateUserRequestDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
