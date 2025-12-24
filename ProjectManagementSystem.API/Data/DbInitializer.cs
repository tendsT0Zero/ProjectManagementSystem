using Microsoft.AspNetCore.Identity;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Static_Details;

namespace ProjectManagementSystem.API.Data
{
    public class DbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
   
            if (!await _roleManager.RoleExistsAsync(SD.UserRoleType.ProjectManager.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.UserRoleType.ProjectManager.ToString()));
                await _roleManager.CreateAsync(new IdentityRole(SD.UserRoleType.TeamLeader.ToString()));
                await _roleManager.CreateAsync(new IdentityRole(SD.UserRoleType.TeamMember.ToString()));
            }

            var adminUser = await _userManager.FindByEmailAsync("admin@project.com");

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin@project.com",
                    Email = "admin@project.com",
                    FullName = "NAYMUR ROHMAN",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(newAdmin, SD.UserRoleType.ProjectManager.ToString());
                }
            }
        }
    }
}
