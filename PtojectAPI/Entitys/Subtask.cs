using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PtojectAPI.FileUploads;

namespace PtojectAPI.Models
{
    public class Subtask
    {

        public int SubtaskId { get; set; }
        public string Task { get; set; }
         
        [Required]
        public int TitleId { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public string? Description { get; set; }    //add on

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        // Navigation properties
        [JsonIgnore]
        public Projectsetup? Projectsetup { get; set; }
        [JsonIgnore]
        public User? AssignedUser { get; set; }


        [JsonIgnore]
        public ICollection<FileUpload> fileUploads { get; set; } = new List<FileUpload>();


        [JsonIgnore]
        public ICollection<LinkUpload> linkUploads { get; set; } = new List<LinkUpload>();

    }

    public enum TaskStatus
    { 
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Canceled = 3,
        OnHold = 4
    }
}
