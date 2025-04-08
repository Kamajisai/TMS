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
    public class MessageController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IHubContext<ChatHub> _chatHub;

        public MessageController(DBContext context, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _chatHub = chatHub;
        }

        // ✅ Send a message
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (message == null)
            {
                return BadRequest("Message cannot be empty");
            }

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // ✅ Notify the receiver in real-time
            await _chatHub.Clients.User(message.ReceiverId.ToString())
                      .SendAsync("ReceiveMessage", message.SenderId, message.Content);

            return Ok(new { message = "Message sent successfully" });
        }

        // ✅ Get all messages between two users
        [HttpGet("chat/{user1Id}/{user2Id}")]
        public IActionResult GetMessages(int user1Id, int user2Id)
        {
            var messages = _context.Messages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.SendAt)
                .ToList();

            return Ok(messages);
        }

        // ✅ Get unread messages for a user
        [HttpGet("unread/{receiverId}")]
        public IActionResult GetUnreadMessages(int receiverId)
        {
            var messages = _context.Messages
                .Where(m => m.ReceiverId == receiverId && m.Status != MessageStatus.Read)
                .ToList();

            return Ok(messages);
        }

        // ✅ Mark a message as "Read"
        [HttpPut("mark-as-read/{messageId}")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            var message = _context.Messages.Find(messageId);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            message.Status = MessageStatus.Read;
            await _context.SaveChangesAsync();

            // ✅ Notify sender that the message is read
            await _chatHub.Clients.User(message.SenderId.ToString()).SendAsync("MessageRead", message.ReceiverId, messageId);

            return Ok(new { message = "Message marked as read" });
        }

        // ✅ Delete a message
        [HttpDelete("delete/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var message = _context.Messages.Find(messageId);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Message deleted successfully" });
        }
    }
}
