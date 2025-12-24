using Microsoft.AspNetCore.Identity;

namespace ProjectManagementSystem.API.Models
{
    public class ApplicationUser:IdentityUser
    {
 
        public string FullName { get; set; }
        public ICollection<Project> LedProjects { get; set; }
        public ICollection<ProjectMember> ProjectMemberships { get; set; }
        public ICollection<TaskItem> AssignedTasks { get; set; }
    }
}
