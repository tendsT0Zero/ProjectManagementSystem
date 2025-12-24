using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.API.Models;

namespace ProjectManagementSystem.API.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> Members { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskSubmission> TaskSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

          
            builder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });


            builder.Entity<Project>()
                .HasOne(p => p.TeamLeader)
                .WithMany(u => u.LedProjects)
                .HasForeignKey(p => p.TeamLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

      
            builder.Entity<TaskItem>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
