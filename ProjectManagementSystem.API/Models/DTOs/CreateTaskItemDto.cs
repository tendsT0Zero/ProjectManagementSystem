using ProjectManagementSystem.API.Static_Details;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models.DTOs
{
    public class CreateTaskItemDto
    {

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        [Required]
        public string? Status { get; set; } = SD.TaskStatus.Pending.ToString();
        public string? Priority { get; set; } = SD.TaskPriority.Low.ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        // Project
        public int ProjectId { get; set; }

        //Assigned User
        public string AssignedToUserId { get; set; }

    }
}
