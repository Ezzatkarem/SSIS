using SSIS.Domain.Common;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Entities
{
    public class User :BaseEntity,ISoftDelete
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        // من ISoftDelete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

    }
}
