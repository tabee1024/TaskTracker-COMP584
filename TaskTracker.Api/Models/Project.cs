using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation property for one-to-many relationship
        // Initialized to avoid null reference
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
