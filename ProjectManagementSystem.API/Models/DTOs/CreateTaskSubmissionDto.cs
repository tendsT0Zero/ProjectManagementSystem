using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models.DTOs
{
    public class CreateTaskSubmissionDto
    {
        public string SubmissionNotes { get; set; }
        public IFormFile? File { get; set; }

        // Task
        public int TaskId { get; set; }
    }
}
