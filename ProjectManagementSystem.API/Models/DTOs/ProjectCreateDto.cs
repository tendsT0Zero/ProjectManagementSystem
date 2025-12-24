using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models.DTOs
{
    public class ProjectCreateDto
    {


        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string? TeamLeaderId { get; set; }

        public ICollection<string>? MembersIds { get; set; }

    }
}
