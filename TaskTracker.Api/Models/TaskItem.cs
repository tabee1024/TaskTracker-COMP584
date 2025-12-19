using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskTracker.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        // Foreign key
        public int ProjectId { get; set; }

        // Navigation property - nullable or ignored for JSON binding
        [JsonIgnore] // Important: prevents model validation error
        public Project? Project { get; set; }
    }
}
