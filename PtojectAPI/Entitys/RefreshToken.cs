using PtojectAPI.Models;

namespace PtojectAPI.Entitys
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;

        // Foreign key to User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
