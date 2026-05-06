using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.DTOs.Notification
{
    public class SendNotificationDto
    {
        
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType NotificationType    { get; set; }


    }
}
