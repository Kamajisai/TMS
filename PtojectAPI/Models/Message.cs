using System.Text.Json.Serialization;

namespace PtojectAPI.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }


        [JsonIgnore]
        public virtual User Sender { get; set; }


        public int ReceiverId { get; set; }
        [JsonIgnore]
        public virtual User Receiver { get; set; }


        public string Content { get; set; }
        public DateTime SendAt { get; set; } = DateTime.UtcNow;
        

        public  MessageStatus Status { get; set; } = MessageStatus.Sent;
    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read
    }
}
