using ProjectManagementSystem.API.Static_Details;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        [Required]
        public string? Status { get; set; } = SD.TaskStatus.Pending.ToString();
        public string? Priority { get; set; } = SD.TaskPriority.Low.ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public bool IsRequiredAttachment { get; set; } = false;
        // Project
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        //Assigned User
        public string AssignedToUserId { get; set; }
        [ForeignKey("AssignedToUserId")]
        public ApplicationUser AssignedToUser { get; set; }

        // Task Submissions
        public ICollection<TaskSubmission>? Submissions { get; set; }
    }
}
