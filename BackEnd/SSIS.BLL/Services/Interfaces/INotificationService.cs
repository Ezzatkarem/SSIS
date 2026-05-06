using SSIS.BLL.DTOs.Notification;
using SSIS.BLL.Responce;
using SSIS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Interfaces
{
    public interface INotificationService
    {

        // Default
        Task SendNotificationAsync(SendNotificationDto dto);
        Task<Responce<IReadOnlyList<NotificationDto>>> GetUserNotificationAsync(Guid Userid);
        Task<Responce<IReadOnlyList<NotificationDto>>> GetUnReadNotificationAsync(Guid UserId);
        Task<int> GetUnReadCountAsync(Guid UserId);
        Task  MarkAsReadAsync(Guid NotificationId, Guid UserId);
        Task  MarkAllAsReadAsync( Guid UserId);



        // Notification for Payment && Fees
        Task NotifyFeeCreatedAsync(Guid studentId, decimal Amount, int semester, int Year);
        Task NotifyFeeReminderAsync(Guid studentId, decimal Amount, DateTime DueDate);
        Task NotifyPaymentSeccessAsync(Guid studentId, decimal Amount,string transctionId);
        Task NotifyPaymentFaildAsync(Guid studentId, decimal Amount, int semester, string ErrorMessage);
        Task NotifyAdminOverduefeesAsync(int Overduecount);


        // Admin

        Task<int> sendAdminBroadcastAsync(AdminBroadcastDto dto);
        Task<int> SendNotificationsByDoctor(DoctorBroadcastDto dto, Guid doctorId);



        // users

        Task NotifyWellcomeAsync(Guid UserId, string username, string userrole);

        Task NotifyUserRegisterAsync(Guid AdminId, string username, string userrole);
        Task NotifyProfileUpdateAsync(Guid Userid, string UpdateField);
        Task NotifyUserDeleteAsync(Guid AdminId, string username);



        // cources
        Task NotifyCourseCreatedAsync(string courceName, string CourceCode);
        Task NotifyCourseUpdatedAsync(string courceName, string CourceCode);
        Task NotifyCourseDeletedAsync(string courceName, string CourceCode);

        // EnroolmentCource
        Task NotifyEnrollmentCreatedAsync(Guid studintid, string CourceName);
        Task NotifyEnrollmentRemovedAsync(Guid studintid, string CourceName);



        // Grade
        Task NotifyGradeEnteredAsync(Guid studintid, string CourceName, decimal Score);
        Task NotifyGradeUpdatedAsync(Guid studentId, string courseName, decimal oldScore, decimal newScore);
        // Attendance

        Task NotifyAttendanceRecordedAsync(Guid studintid, string CourceName, string status);













    }
}
