using SSIS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.Domain.Interfaces
{
    public interface INotficationRepo:IRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetNotificationsByUserIdAsync(Guid userId);
        Task<IReadOnlyList<Notification>> GetUnreadNotificationsByUserIdAsync(Guid userId);
        Task<int> GetUnreadNotificationsCountByUserIdAsync(Guid UserId);
        Task markAsReadAsync(Guid notificationId);
        Task markAllAsreadAsync(Guid UserId);
    }
}
