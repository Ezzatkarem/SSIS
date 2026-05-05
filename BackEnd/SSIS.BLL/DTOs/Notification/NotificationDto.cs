using Org.BouncyCastle.Bcpg.OpenPgp;
using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Notification
{
    public class NotificationDto
    {
        public Guid id {  get; set; }
        public string title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreateAt { get; set; }
        public NotificationType NotificationType { get; set; }

        public DateTime ReadAt { get; set; }
    }
}
