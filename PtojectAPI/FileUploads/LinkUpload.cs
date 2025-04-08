using System.ComponentModel.DataAnnotations;
using PtojectAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PtojectAPI.FileUploads
{
    public class LinkUpload
    {
        public int LinkId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubtaskId { get; set; }

        public string Url { get; set; }         //link   url

        public DateTime InsertDate { get; set; }

        [JsonIgnore]
        public User? UploadUser { get; set; }


        [JsonIgnore]
        public Subtask? Subtask { get; set; }
    }
}
