using SSIS.Domain.Common;
using SSIS.Domain.Enum;
namespace SSIS.Domain.Entities
{
    public class User : BaseEntity, ISoftDelete
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // مسار الملف (يحتوي على البطاقة والشهادة)
        public string? DocumentsFilePath { get; set; }

        // حالة التوثيق
        public bool IsVerified { get; set; } = false;

        // ✅ ربط مع Identity User (لما نستخدم Identity)
        public string IdentityUserId { get; set; } = string.Empty;
    }
}