using Microsoft.AspNetCore.SignalR;
using PtojectAPI.Models;
using PtojectAPI.Datas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PtojectAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly DBContext _context;

        public ChatHub(DBContext context)
        {
            _context = context;
        }

        private static readonly ConcurrentDictionary<int, string> _userConnections = new();
        private static readonly ConcurrentDictionary<int, bool> _userOnlineStatus = new();
        private static readonly ConcurrentDictionary<int, List<Message>> _undeliveredMessages = new();

        // ✅ When a user connects, store their connection ID
        public override async Task OnConnectedAsync()
        {
            if (Context.GetHttpContext().Request.Query.TryGetValue("userId", out var userIdString) &&
                int.TryParse(userIdString, out int userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                _userOnlineStatus[userId] = true;

                // ✅ Deliver any undelivered messages
                if (_undeliveredMessages.TryRemove(userId, out var messages))
                {
                    foreach (var msg in messages)
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", msg.SenderId, msg.Content);
                        msg.Status = MessageStatus.Delivered;
                    }
                    await _context.SaveChangesAsync();
                }

                // ✅ Notify all users that this user is online
                await Clients.All.SendAsync("UserOnlineStatus", userId, true);
            }
            await base.OnConnectedAsync();
        }

        // ✅ When a user disconnects, mark them as offline
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != 0)
            {
                _userConnections.TryRemove(userId, out _);
                _userOnlineStatus[userId] = false;

                // ✅ Notify all users that this user is offline
                await Clients.All.SendAsync("UserOnlineStatus", userId, false);
            }
            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Real-time messaging
        public async Task SendMessage(int senderId, int receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Status = MessageStatus.Sent
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            if (_userConnections.TryGetValue(receiverId, out string receiverConnectionId))
            {
                message.Status = MessageStatus.Delivered;
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, content);
            }
            else
            {
                // ✅ Store undelivered messages if user is offline
                if (!_undeliveredMessages.ContainsKey(receiverId))
                    _undeliveredMessages[receiverId] = new List<Message>();

                _undeliveredMessages[receiverId].Add(message);
            }
            await _context.SaveChangesAsync();
        }

        // ✅ User starts typing
        public async Task StartTyping(int senderId, int receiverId)
        {
            if (_userConnections.TryGetValue(receiverId, out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("UserTyping", senderId);
            }
        }

        // ✅ Mark message as "Read"
        public async Task MarkMessageAsRead(int messageId)
        {
            var message = _context.Messages.Find(messageId);
            if (message != null)
            {
                message.Status = MessageStatus.Read;
                await _context.SaveChangesAsync();

                if (_userConnections.TryGetValue(message.SenderId, out string senderConnectionId))
                {
                    await Clients.Client(senderConnectionId).SendAsync("MessageRead", message.ReceiverId, messageId);
                }
            }
        }

        // ✅ Check if a user is online
        public bool IsUserOnline(int userId) => _userOnlineStatus.TryGetValue(userId, out bool isOnline) && isOnline;
    }
}
