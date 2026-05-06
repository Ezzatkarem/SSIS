using SSIS.Domain.Common;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public bool IsDeleted { get ; set ; }
        public DateTime? DeletedAt { get; set ; }
        public string? DeletedBy { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }=string.Empty;
        public string Message {  get; set; }=string.Empty;
        public bool IsRead { get; set; }
        public DateTime ReadAt { get; set; }
        public NotificationType NotificationType { get; set; }
        public virtual User User { get; set; }=null!;
    }
}
