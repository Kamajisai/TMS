

namespace PtojectAPI.Entitys
{
    public class TaskDto
    {
       public string TaskName { get; set; }

        public string Description { get; set; }

        public int AssignedTo { get; set; }      //user Id based get 

        public DateTime Duedate { get; set; }

        public string status { get; set; }

        public int DocumentId { get; set; }

        public int LinkId { get; set; }

        public string ProjectHead { get; set; }
        
        public string ProjectName   { get; set; } 
    }
}
