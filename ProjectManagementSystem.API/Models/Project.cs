using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string? TeamLeaderId { get; set; }

        [ForeignKey("TeamLeaderId")]
        public ApplicationUser? TeamLeader { get; set; }
        public ICollection<ProjectMember>? Members { get; set; }
        public ICollection<TaskItem>? Tasks { get; set; }
    }
}
