using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PtojectAPI.Datas;
using PtojectAPI.Hubs;
using PtojectAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PtojectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public NotificationController(DBContext context, IHubContext<NotificationHub> notificationHub)
        {
            _context = context;
            _notificationHub = notificationHub;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] Notification notification)
        {
            if (notification == null || string.IsNullOrEmpty(notification.Message))
            {
                return BadRequest("Invalid notification.");
            }

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // ✅ Send real-time notification
            await _notificationHub.Clients.User(notification.UserId.ToString())
                .SendAsync("ReceiveNotification", notification.Message);

            return Ok(new { message = "Notification sent successfully." });
        }

        [HttpGet("{userId}")]
        public IActionResult GetNotifications(int userId)
        {
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return Ok(notifications);
        }
    }
}
