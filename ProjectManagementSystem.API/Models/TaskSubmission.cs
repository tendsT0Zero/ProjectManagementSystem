using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models
{
    public class TaskSubmission
    {
        [Key]
        public int Id { get; set; }
        public string SubmissionNotes { get; set; }
        public string AttachmentUrl { get; set; }

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;

        // Task
        public int TaskId { get; set; }
        [ForeignKey("TaskId")]
        public TaskItem Task { get; set; }
        public string SubmitterId { get; set; }
        [ForeignKey("SubmitterId")]
        public ApplicationUser Submitter { get; set; }
    }
}
