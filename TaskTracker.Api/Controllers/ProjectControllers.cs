using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProjectsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _db.Projects
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

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        // GET: /api/Projects/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            var project = await _db.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();
            return Ok(project);
        }

        // DELETE: /api/Projects/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
