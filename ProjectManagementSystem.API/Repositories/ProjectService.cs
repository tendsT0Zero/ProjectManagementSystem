using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.API.Data;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Static_Details;

namespace ProjectManagementSystem.API.Repositories
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region Create Project
        public async Task<ResponseDto> CreateAsync(Project project)
        {
            try
            {
                var existingProject = await _context.Projects.FirstOrDefaultAsync(x => x.Name == project.Name);
                if (existingProject != null)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Project with this name already exists."
                    };
                }

                project.CreatedAt = DateTime.UtcNow;


                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();

                return new ResponseDto
                {
                    IsSuccess = true,
                    ResponseObject = project,
                    ErrorMessage = "Project Created Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Update Project
        public async Task<ResponseDto> UpdateAsync(Project project)
        {
            try
            {
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();

                return new ResponseDto
                {
                    IsSuccess = true,
                    ResponseObject = project
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Delete Project
        public async Task<ResponseDto> DeleteAsync(int id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);
                if (project == null)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found." };
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                return new ResponseDto { IsSuccess = true, ErrorMessage = "Project Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Get By ID
        public async Task<ResponseDto> GetProjectById(int id)
        {
            try
            {

                var project = await _context.Projects
                    .Include(p => p.TeamLeader)
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found." };
                }

                return new ResponseDto { IsSuccess = true, ResponseObject = project };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Get All (Pagination & Sorting)
        public async Task<ResponseDto> GetAllProjectsAsync(int pageNumber, int pageSize, string sort)
        {
            try
            {
                var query = _context.Projects.Include(p => p.TeamLeader).AsQueryable();
                

                if (!string.IsNullOrEmpty(sort) && sort.ToLower() == "desc")
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }


                var totalCount = await query.CountAsync();
                if (totalCount == 0)
                {
                    return new()
                    {
                        ResponseObject=null,
                        IsSuccess=true,
                        ErrorMessage="No Projects Availble now."
                    };
                }
                var projects = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();


                var paginationResult = new
                {
                    TotalCount = totalCount,
                    Page = pageNumber,
                    Size = pageSize,
                    Data = projects
                };

                return new ResponseDto { IsSuccess = true, ResponseObject = paginationResult };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Assign Team Leader
        public async Task<ResponseDto> AssignTeamLeaderAsync(int projectId, string teamLeaderId)
        {
            try
            {
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null) return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found" };

                var user = await _userManager.FindByIdAsync(teamLeaderId);
                if (user == null) return new ResponseDto { IsSuccess = false, ErrorMessage = "User not found" };

                var isTeamLeader = await _userManager.IsInRoleAsync(user, SD.UserRoleType.TeamLeader.ToString());
                if (!isTeamLeader) return new ResponseDto { IsSuccess = false, ErrorMessage = "User is not a Team Leader" };

                project.TeamLeaderId = teamLeaderId;
                await _context.SaveChangesAsync();

                return new ResponseDto { IsSuccess = true, ErrorMessage = "Team Leader Assigned Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        #region Add Team Members
        public async Task<ResponseDto> AddTeamMembersAsync(int projectId, List<string> memberIds)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                if (project == null) return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found" };

                foreach (var userId in memberIds)
                {

                    if (project.Members.Any(pm => pm.UserId == userId)) continue;

                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null) continue;


                    await _context.Members.AddAsync(new ProjectMember
                    {
                        ProjectId = projectId,
                        UserId = userId
                    });
                }

                await _context.SaveChangesAsync();
                return new ResponseDto { IsSuccess = true, ErrorMessage = "Members Added Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        #endregion

        //Get projects with projectId and TeamLeaderId
        public async Task<ResponseDto> GetProjectsByTeamLeaderIdAsync(int projectId,string teamLeaderId)
        {
            try
            {
                var project = await _context.Projects.Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.TeamLeaderId == teamLeaderId);
                    
                if (project == null)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Project not found or you are not the owner."
                    };
                }
                return new ResponseDto { IsSuccess = true, ResponseObject = project };
            }
            catch (Exception ex)
            {
                return new ResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<ResponseDto> GetAllProjectDetailsAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.TeamLeader)
                .Include(p => p.Members)
                .ThenInclude(m => m.User)
                .ToListAsync();
            if (projects.Count == 0)
            {
                return new ResponseDto
                {
                    IsSuccess = true,
                    ResponseObject = null,
                    ErrorMessage = "No Projects Available now."
                };
            }
            var projectDetailsList = projects.Select(project => new ProjectDetailsDto
            {
                ProjectName = project.Name,
                TeamLeaderName = project.TeamLeader != null ? project.TeamLeader.FullName : "No Team Leader Assigned",
                MemberNames = project.Members != null
                        ? project.Members
                            .Where(m => m.User != null)
                            .Select(m => m.User.FullName)
                            .ToList()
                        : new List<string>()
            }).ToList();
            return new ResponseDto
            {
                IsSuccess = true,
                ResponseObject = projectDetailsList
            };

        }
    }
}