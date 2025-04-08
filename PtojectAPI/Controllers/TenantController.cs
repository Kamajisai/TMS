using System.Security.Claims;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PtojectAPI.Datas;
using PtojectAPI.Entitys;

namespace PtojectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly DBContext _dbContext;
        public TenantController(DBContext dbContext)
        { 
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
        {
            var tenants = await _dbContext.Tenants.ToListAsync();               
            return Ok(tenants);
        }

        [HttpGet("GetTenant")]
        [Authorize] 
        public async Task<ActionResult<Tenant>> GetTenant()
        {
            var userIdClaim = User.Claims
                                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token. User ID not found or incorrect format." });
            }


            var tenants = await _dbContext.Tenants
        .Where(t => t.UserId == userId)
        .ToListAsync();

            if (tenants == null || !tenants.Any())
            {
                return NotFound();
            }
            return Ok(tenants);
        }

        [HttpPost]
        public async Task<ActionResult<Tenant>> CreateOrganization([Bind("Name,Email,UserId")] Tenant tenant)
        {
            if (tenant == null || tenant.UserId == 0)
            {
                return BadRequest("Invalid Tenant data. UserId is required.");
            }

            var user = await _dbContext.Users.FindAsync(tenant.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            tenant.User = user; // Attach user from DB
            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.TenantId }, tenant);
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTenant(int id, Tenant tenant)
        //{
        //    if (id != tenant.TenantId)
        //    {
        //        return BadRequest();
        //    }

        //    var existingTenant = await _dbContext.Tenants.FindAsync(id);
        //    if (existingTenant == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update fields (avoid modifying navigation properties)
        //    existingTenant.Name = tenant.Name;
        //    existingTenant.Email = tenant.Email;

        //    await _dbContext.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
