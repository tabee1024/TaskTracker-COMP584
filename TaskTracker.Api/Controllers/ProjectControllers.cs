using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private async Task<IdentityUser> GetCurrentUser()
        {
            // Your token sets "sub" = username
            var username =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
                throw new UnauthorizedAccessException("Not authenticated.");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            return user;
        }

        // GET: /api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var user = await GetCurrentUser();

            var projects = await _db.Projects
                .Where(p => p.OwnerId == user.Id)
                .Include(p => p.Tasks)
                .ToListAsync();

            return Ok(projects);
        }

        // POST: /api/Projects
        [HttpPost]
        public async Task<ActionResult<Project>> Create([FromBody] Project project)
        {
            if (string.IsNullOrWhiteSpace(project.Name))
                return BadRequest("Project name is required.");

            var user = await GetCurrentUser();

            project.OwnerId = user.Id;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return Ok(project);
        }

        // GET: /api/Projects/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            var user = await GetCurrentUser();

            var project = await _db.Projects
                .Where(p => p.OwnerId == user.Id)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        // DELETE: /api/Projects/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await GetCurrentUser();

            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == user.Id);

            if (project == null)
                return NotFound();

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
