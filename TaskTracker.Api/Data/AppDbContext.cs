using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
