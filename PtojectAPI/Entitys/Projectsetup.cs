using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using PtojectAPI.Entitys;

namespace PtojectAPI.Models
{
    public class Projectsetup
    {
        public int TitleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int UserId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Tenant? Tenant { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<Subtask> Tasks { get; set; } = new List<Subtask>();





        //[Key]
        //public int TitleId { get; set; }  // project ID 

        //public string Title { get; set; } // project name

        //public string? Description { get; set; }

        //[Required]
        //public int TenantId { get; set; } // Project belongs to a Tenant

        //[ForeignKey("TenantId")]
        //public Tenant Tenant { get; set; }

        //[Required]
        //public int UserId { get; set; } // Person in charge of the project

        //[ForeignKey("UserId")]
        //public User User { get; set; } // The User managing the project



        //// A Project can have multiple Subtasks
        //public ICollection<Subtask> Tasks { get; set; }

    }
}
