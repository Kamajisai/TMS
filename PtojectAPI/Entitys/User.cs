using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PtojectAPI.Entitys;
using PtojectAPI.FileUploads;

namespace PtojectAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();

        [JsonIgnore]
        public ICollection<Projectsetup> Projects { get; set; } = new List<Projectsetup>();

        [JsonIgnore]
        public ICollection<Subtask> AssignedTasks { get; set; } = new List<Subtask>();
        [JsonIgnore]
        public ICollection<FileUpload> UserFiles { get; set; } = new List<FileUpload>();
        [JsonIgnore]
        public ICollection<LinkUpload> UserLinks { get; set; } = new List<LinkUpload>();
        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


        [JsonIgnore]
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        [JsonIgnore]
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        [JsonIgnore]
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    }
}
