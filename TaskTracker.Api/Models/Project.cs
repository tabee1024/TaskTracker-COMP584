using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Api.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string OwnerId { get; set; } = string.Empty;


        [JsonIgnore]
        public IdentityUser? Owner { get; set; }

        // One-to-many: Project → Tasks
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
