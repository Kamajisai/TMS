using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PtojectAPI.Models;

namespace PtojectAPI.Entitys
{
    public class Tenant
    {

        public int TenantId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public int UserId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<Projectsetup> Projects { get; set; } = new List<Projectsetup>();

        //[Key]
        //public int TenantId { get; set; } // Unique Tenant ID

        //[Required]
        //public string Name { get; set; } // Tenant Name (Company/Organization)

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; } // Admin/Contact Email

        //[Required]
        //public int UserId { get; set; }  // Owner of the Tenant

        //[ForeignKey("UserId")]
        //public User User { get; set; } // Tenant is owned by a User


        //// A Tenant can have multiple Projects
        //public ICollection<Projectsetup> Projectsetups { get; set; }

    }
}
