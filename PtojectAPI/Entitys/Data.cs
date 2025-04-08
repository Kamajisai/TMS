using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PtojectAPI.Models
{
    public class Data
    {
        public List<Data> ListModel = new List<Data>();
        public List<SelectListItem> ListEmail = new List<SelectListItem>();

        public string process_td_tms_id { get; set; }
        public string process_date { get; set; }
        public string process_content { get; set; }
        public string process_colour { get; set; }

        [DisplayName("Start")]
        public string StartDate { get; set; }

        [DisplayName("End")]
        public string EndDate { get; set; }

        public int nDay { get; set; }


        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage ="You need to give Project Title")]
        public string name { get; set; }

        [DisplayName("Pic")]
        [Required(ErrorMessage ="you need to give Person Incharge")]
        public string pic { get; set; }

        public string pic3 { get; set; }
        public string collaborate { get; set; }
        public string spec2 { get; set; }


        [DisplayName("Email")]
        [Required(ErrorMessage = "You need Email for your Project ")]
        public string email1 { get; set; }
        public string email2 { get; set; }
        
        [DisplayName("Group")]
        [Required(ErrorMessage ="You need Group for your Project")]
        public string spec { get; set; }
        [DisplayName("Start Date")]
        [Required(ErrorMessage ="You need Start Date for Your Project")]
        public string registerdate { get; set; }

        [DisplayName("Due Date")]
        [Required(ErrorMessage = "You need Due Date for Your Project")]
        public string duedate { get; set; }
        public string duedate2 { get; set; }

        [DisplayName("Remark")]
        public string remark { get; set; }

        [DisplayName("ID")]
        public string td_tms_id { get; set; }
    }
}
