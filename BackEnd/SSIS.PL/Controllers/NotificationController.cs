
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paymob.Net.Models;
using SSIS.BLL.DTOs.Notification;
using SSIS.BLL.Services.Implementaion;
using SSIS.BLL.Services.Interfaces;
using SSIS.Domain.Entities;

namespace SSIS.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly IUserService userService;

        public NotificationController(INotificationService notificationService, IUserService userService)
        {
            this.notificationService = notificationService;
            this.userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var notifications = notificationService.GetUserNotificationAsync(userid);
            return Ok(notifications);

        }
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var notifications = notificationService.GetUnReadNotificationAsync(userid);
            return Ok(notifications);

        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCountNotifications()
        {
            var userid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var notifications = notificationService.GetUnReadCountAsync(userid);
            return Ok(notifications);

        }
        [HttpPut("{notificationid}/read")]
        public async Task<IActionResult> GetUnreadNotifications(Guid notificationid)
        {
            var userid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var notifications = notificationService.MarkAsReadAsync(notificationid, userid);
            return Ok(notifications);

        }
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userid = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            var notifications = notificationService.MarkAllAsReadAsync(userid);
            return Ok(notifications);

        }

        [HttpPost("broadcast")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendBroadcast([FromBody] AdminBroadcastDto dto)
        {
            if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Message))
                return BadRequest(new { message = "Title and Message are required" });

            var count = await notificationService.sendAdminBroadcastAsync(dto);
            await notificationService.sendAdminBroadcastAsync(dto);

            return Ok(new { message = $"Broadcast sent to {count} user(s)", recipientsCount = count });
        }


        [HttpPost("broadcast-by-doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SendBroadcastByDoctor([FromBody] DoctorBroadcastDto dto)
        {

            var doctorId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Message))
                return BadRequest(new { message = "Title and Message are required" });

            var count = await notificationService.SendNotificationsByDoctor(dto, doctorId);
            await notificationService.SendNotificationsByDoctor(dto,doctorId);


            return Ok(new { message = $"Broadcast sent to {count} student(s)", recipientsCount = count });
        }
    }
}
