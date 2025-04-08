using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtojectAPI.Datas;
using PtojectAPI.Entitys;
using PtojectAPI.Hubs;
using PtojectAPI.Models;

namespace PtojectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly DBContext _dbContext;

        private readonly IHubContext<NotificationHub> _notificationHub;
        public TaskController(DBContext dbContext, IHubContext<NotificationHub> notificationHub)
        {
            _dbContext = dbContext;
            _notificationHub = notificationHub;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subtask>>> GetTasks()
        {
            var tasks = await _dbContext.Tasks.ToListAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<Subtask>> CreateTask([Bind("Task, TitleId, UserId")] Subtask subtask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var title = await _dbContext.Projects.FindAsync(subtask.TitleId);
            if (title == null)
            {
                return BadRequest("No Projects found");
            }
            subtask.Projectsetup = title;

            var user = await _dbContext.Users.FindAsync(subtask.UserId);
            if (user == null)
            {
                return BadRequest("No user found with Id");
            }
            subtask.AssignedUser = user;

            _dbContext.Tasks.Add(subtask);
            await _dbContext.SaveChangesAsync(); // ✅ Save task to DB

            // ✅ Create a new notification for the assigned user
            var notification = new Notification
            {
                UserId = subtask.UserId,
                Message = $"You have been assigned a new task: {subtask.Task}",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync(); // ✅ Save notification to DB

            // ✅ Send real-time notification using SignalR
            await _notificationHub.Clients.User(subtask.UserId.ToString())
                .SendAsync("ReceiveNotification", notification.Message);

            return CreatedAtAction(nameof(GetTasks), new { id = subtask.SubtaskId }, subtask);
        }


        //[HttpPost]
        //public async Task<ActionResult<Subtask>> CreateTask([Bind("Task, TitleId, UserId")] Subtask subtask)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var title = await _dbContext.Projects.FindAsync(subtask.TitleId);
        //    if (title == null)
        //    {
        //        return BadRequest("No Projects found");
        //    }
        //    subtask.Projectsetup = title;
        //    var user = await _dbContext.Users.FindAsync(subtask.UserId);
        //    if (user == null)
        //    {
        //        return BadRequest("No user found with Id");
        //    }
        //    subtask.AssignedUser = user;
        //    _dbContext.Tasks.Add(subtask);
        //    await _dbContext.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetTasks), new { id = subtask.SubtaskId }, subtask);
        //}

        //[HttpGet("{Id}")]
        //public async Task<ActionResult<IEnumerable<TaskDto>>> GetHomeTasks(int Id)
        //{
        //    try
        //    {
        //        var tasks = await _dbContext.Tasks
        //            .Where(t => t.UserId == Id)
        //            .Select(t => new TaskDto
        //            {
        //                TaskName = t.Task,
        //                Description = t.Description,
        //                AssignedTo = t.UserId,
        //                Duedate = t.DueDate
        //            })
        //            .ToListAsync();

        //        return Ok(tasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error: {ex.Message}");
        //    }
        //}


        //[HttpPut("UpdateTaskHome{Id}")]
        //public async Task<ActionResult<Subtask>> UpdateTask(int Id, Subtask subtask)
        //{ 
        //    var user = await _dbContext.Tasks.SingleOrDefault    
        //}

        //[HttpGet("{Id}")]
        //public async Task<ActionResult<IEnumerable<TaskDto>>> GetHomeTasks(int Id)
        //{
        //    var tasks = await (from t in _dbContext.Tasks
        //                       join d in _dbContext.Files
        //                            on t.SubtaskId equals d.SubtaskId
        //                       join l in _dbContext.Links
        //                            on t.SubtaskId equals l.SubtaskId
        //                       join p in _dbContext.Projects
        //                           on t.TitleId equals p.TitleId
        //                       join u in _dbContext.Users
        //                           on p.UserId equals u.UserId
        //                       where t.UserId == Id
        //                       select new TaskDto
        //                       {
        //                           TaskName = t.Task,
        //                           Description = t.Description,
        //                           AssignedTo = t.UserId,
        //                           Duedate = t.DueDate,
        //                           //status = t.Status,
        //                           DocumentId = d != null ? d.DocumentId : 0,
        //                           LinkId = l != null ? l.LinkId : 0,
        //                           ProjectName = p != null ? p.Title : "No Project",
        //                           ProjectHead = u != null ? u.Name : "No Head",
        //                       }).ToListAsync();

        //    return Ok(tasks);
        //}



        [HttpGet("GetHomeTasks")]
        [Authorize] 
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetHomeTasks()
        {
            try
            {
                // Extract logged-in user's ID from claims
                //var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                //var userIdClaim = User.FindFirst("nameid")?.Value;
                //if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                //{
                //    return Unauthorized(new { message = "Invalid token. User ID not found or incorrect format." });
                //}

                var userIdClaim = User.Claims
                                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token. User ID not found or incorrect format." });
                }



                var tasks = await (from t in _dbContext.Tasks

                                   join d in _dbContext.Files
                                        on t.SubtaskId equals d.SubtaskId into fileGroup
                                   from d in fileGroup.DefaultIfEmpty()  // Left join

                                   join l in _dbContext.Links
                                        on t.SubtaskId equals l.SubtaskId into linkGroup
                                   from l in linkGroup.DefaultIfEmpty()  // Left join

                                   join p in _dbContext.Projects
                                       on t.TitleId equals p.TitleId into projectGroup
                                   from p in projectGroup.DefaultIfEmpty()  // Left join

                                   join u in _dbContext.Users
                                       on p.UserId equals u.UserId into userGroup
                                   from u in userGroup.DefaultIfEmpty()  // Left join

                                   where t.UserId == userId
                                   select new TaskDto
                                   {
                                       TaskName = t.Task,
                                       Description = t.Description,
                                       AssignedTo = t.UserId,
                                       Duedate = t.DueDate,
                                       status = t.Status.ToString(),
                                       DocumentId = d != null ? d.DocumentId : 0,
                                       LinkId = l != null ? l.LinkId : 0,
                                       ProjectName = p != null ? p.Title : "No Project",
                                       ProjectHead = u != null ? u.Name : "No Head",
                                   }).ToListAsync();

                if (tasks == null || !tasks.Any())
                {
                    return Ok(new { message = "No tasks found for the user." });
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPatch("updateHomeTasks")]
        public async Task<ActionResult<TaskDto>> updateHomeTasks(TaskDto task)
        {
            var userIdClaim = User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token. User ID not found or incorrect format." });
            }
            var user = await _dbContext.Tasks.SingleOrDefaultAsync(t => t.UserId == userId);
        }

    }
}
