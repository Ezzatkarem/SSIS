using Microsoft.AspNetCore.Identity;
using SSIS.Domain.Entities;

namespace SSIS.DAL.Identity
{
    public class ApplicationUser : IdentityUser<Guid>  // ✅ تأكد من <Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public string? DocumentsFilePath { get; set; }
        public bool IsVerified { get; set; } = false;

        public Guid? DomainUserId { get; set; }
        public virtual User? DomainUser { get; set; }
    }
}