using ProjectManagementSystem.API.Static_Details;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models.DTOs
{
    public class TaskItemResponseDto
    {


        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        [Required]
        public string? Status { get; set; } = SD.TaskStatus.Pending.ToString();
        public string? Priority { get; set; } = SD.TaskPriority.Low.ToString();
        public DateTime? DueDate { get; set; }
        public bool IsRequiredAttachment { get; set; } = false;
        public string ProjectName { get; set; }=string.Empty;

        //Assigned User
        public string AssignedToUserName { get; set; }=string.Empty;
    }
}
