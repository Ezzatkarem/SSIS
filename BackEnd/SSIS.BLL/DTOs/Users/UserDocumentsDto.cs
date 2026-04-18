using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Users
{
    public class UserDocumentsDto
    {
        public string? DocumentsUrl { get; set; }
        public bool IsVerified { get; set; }
    }
}
