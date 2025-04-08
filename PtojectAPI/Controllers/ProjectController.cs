using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtojectAPI.Datas;
using PtojectAPI.Entitys;
using PtojectAPI.Models;

namespace PtojectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly DBContext _dbcontext;

        public ProjectController(DBContext context)
        {
            _dbcontext = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Projectsetup>>> GetProjects()
        {
            var projects = await _dbcontext.Projects.ToListAsync();
            return Ok(projects);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Projectsetup>>> GetProject(int id)
        {

            var projects = await _dbcontext.Projects.Where(p => p.TenantId == id)
                            .ToListAsync();

            return Ok(projects);
        }



        //[HttpGet("{id}")]
        //public async Task<ActionResult<Projectsetup>> GetTaskById(int id)
        //{
        //    var projectset = await _context.projects
        //        .Include(t => t.tasks)
        //        .FirstOrDefaultAsync(t => t.TitleId == id);

        //    if (projectset == null)
        //        return NotFound();

        //    return Ok(projectset);
        //}






        [HttpPost]
        public async Task<ActionResult<Projectsetup>> CreateProject([Bind("Title,Description,TenantId,UserId")] Projectsetup projectset)
        {
            if (projectset == null)
            {
                return BadRequest("Invalid project Data");
            }
            var tenant = await _dbcontext.Tenants.FindAsync(projectset.TenantId);
            if (tenant == null)
            {
                return BadRequest("Organization not found");
            }
            projectset.Tenant = tenant;
            var user = await _dbcontext.Users.FindAsync(projectset.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            projectset.User = user;
            _dbcontext.Projects.Add(projectset);
            await _dbcontext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProjects), new { id = projectset.TitleId }, projectset);
        }
       
        
        
        
        
        
        
        //[HttpPost("{projectId}/subtasks")]
        //public async Task<ActionResult<Subtask>> CreateSubTask(int projectId, [FromBody] Subtask subTask)
        //{
        //    var sub = await _context.projects.FindAsync(projectId);

        //    if (sub == null)
        //        return NotFound("Task not found.");
        //    subTask.TitleId = projectId;
        //    _context.tasks.Add(subTask);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetTaskById), new { id = projectId }, subTask);
        //}


    }
}
