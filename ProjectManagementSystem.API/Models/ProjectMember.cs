using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagementSystem.API.Models
{
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
