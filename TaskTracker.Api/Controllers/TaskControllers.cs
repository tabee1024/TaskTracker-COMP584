using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TasksController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
        {
            var tasks = await _db.Tasks.ToListAsync();
            return Ok(tasks);
        }

        // GET: /api/Tasks/project/3
        [HttpGet("project/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetForProject(int projectId)
        {
            var tasks = await _db.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            return Ok(tasks);
        }

        // POST: /api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("Task title is required.");

            // validate Project exists
            var exists = await _db.Projects.AnyAsync(p => p.Id == task.ProjectId);
            if (!exists) return BadRequest("Invalid ProjectId.");

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        // GET: /api/Tasks/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskItem>> GetById(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // PUT: /api/Tasks/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem updated)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            task.Title = updated.Title;
            task.IsCompleted = updated.IsCompleted;
            task.ProjectId = updated.ProjectId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/Tasks/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
