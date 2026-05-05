using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Data;
using SSIS.DAL.Migrations;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.DAL.Repositories
{
    public class NotficationRepo : Repository<Notification>, INotficationRepo
    {
        public NotficationRepo(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetNotificationsByUserIdAsync(Guid userId)
        {
            return await _context.Notifications.Where(p=>p.UserId==userId)
                .OrderByDescending(p=>p.CreatedAt).ToListAsync();
        }

        public async Task<IReadOnlyList<Notification>> GetUnreadNotificationsByUserIdAsync(Guid userId)
        {
            return await _context.Notifications.Where(p => p.UserId == userId&& p.IsRead==false)
                .OrderByDescending(p=>p.CreatedAt).ToListAsync();

        }

        public async Task<int> GetUnreadNotificationsCountByUserIdAsync(Guid UserId)
        {
            return await _context.Notifications.CountAsync(p => p.UserId == UserId && p.IsRead == false);
        }

        public async Task markAllAsreadAsync(Guid UserId)
        {
           var notifications=await _context.Notifications.Where(p=>p.UserId==UserId).ToListAsync();
            foreach (var notification in notifications)
            {
                 notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
            }
           
            await _context.SaveChangesAsync();

        }

        public async Task markAsReadAsync(Guid notificationId)
        {
            var notification =await _context.Notifications.FirstOrDefaultAsync(p=>p.Id== notificationId);

            if(!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await UpdateAsync(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
