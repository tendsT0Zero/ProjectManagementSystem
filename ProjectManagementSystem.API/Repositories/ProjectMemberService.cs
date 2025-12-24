using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.API.Data;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectMemberService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ResponseDto> UpdateProjectMembersAsync(int projectId, UpdateProjectMembersDto dto)
        {
            try
            {
                
                var project = await _context.Projects
                    .Include(p => p.Members) 
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                if (project == null)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found" };
                }
                    

                var validNewMemberIds = new List<string>();

                var uniqueInputIds = dto.MembersIds.Distinct().ToList();

                foreach (var userId in uniqueInputIds)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null && await _userManager.IsInRoleAsync(user, "TeamMember"))
                    {
                        validNewMemberIds.Add(userId);
                    }
                }

          
                var membersToRemove = project.Members
                    .Where(existing => !validNewMemberIds.Contains(existing.UserId))
                    .ToList();

               
                var currentDbIds = project.Members.Select(m => m.UserId).ToList();
                var idsToAdd = validNewMemberIds
                    .Where(newId => !currentDbIds.Contains(newId))
                    .ToList();

               
                if (membersToRemove.Any())
                {
                    _context.Members.RemoveRange(membersToRemove);
                }

                foreach (var newId in idsToAdd)
                {
                    await _context.Members.AddAsync(new ProjectMember
                    {
                        ProjectId = projectId,
                        UserId = newId
                    });
                }

                await _context.SaveChangesAsync();

                return new ResponseDto
                {
                    IsSuccess = true,
                    ErrorMessage = $"Updated successfully. Added {idsToAdd.Count}, Removed {membersToRemove.Count}."
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<ResponseDto> AddNewMemberToProjectAsync(int projectId, string memberId)
        {
            //check if project exists
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found" };
            }
            //check if user exists and is team member
            var user = await _userManager.FindByIdAsync(memberId);
            if (user == null || !await _userManager.IsInRoleAsync(user, "TeamMember"))
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = "User not found or is not a Team Member" };
            }
            //check if user is already a member
            if (project.Members.Any(m => m.UserId == memberId))
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = "User is already a member of the project" };
            }
            //add new member
            var newMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = memberId
            };
            await _context.Members.AddAsync(newMember);
            await _context.SaveChangesAsync();
            return new ResponseDto { IsSuccess = true, ErrorMessage = "Member added successfully" };
        }
    }
}
