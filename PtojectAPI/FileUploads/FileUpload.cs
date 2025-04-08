using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using PtojectAPI.Models;

namespace PtojectAPI.FileUploads
{
    public class FileUpload
    {
        public int DocumentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubtaskId { get; set; }

        [MaxLength(4000)]
        public string DocumentOriginalFile { get; set; }
       
        [MaxLength(255)]
        public string DocumentName { get; set; }

        [MaxLength(2083)]
        public string DocumentUrlPath { get; set; }

        [DataType(DataType.Date)]
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public  IFormFile   File { get; set; }

        [JsonIgnore]
        public User? UploadUser { get; set; }

        [JsonIgnore]
        public Subtask?  Subtask { get; set; }

    }
}
