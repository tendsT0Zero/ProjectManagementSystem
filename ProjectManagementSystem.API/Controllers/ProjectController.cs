using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Repositories;
using System.Net;
using System.Security.Claims;

namespace ProjectManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        //Create Project
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProjectCreateDto projectCreateDto)
        {

            if (ModelState.IsValid)
            {
                var projectDomain = new Project
                {
                    Name = projectCreateDto.Name,
                    Description = projectCreateDto.Description,
                    Deadline = projectCreateDto.Deadline,
                    CreatedAt = projectCreateDto.CreatedAt,
                    TeamLeaderId = projectCreateDto.TeamLeaderId

                };

                if (projectCreateDto.MembersIds != null && projectCreateDto.MembersIds.Count > 0)
                {
                    foreach (var memberId in projectCreateDto.MembersIds)
                    {
                        projectDomain.Members ??= new List<ProjectMember>();
                        projectDomain.Members.Add(new ProjectMember
                        {
                            UserId = memberId
                        });
                    }
                }

                return Ok(await _projectService.CreateAsync(projectDomain));
            }
            return BadRequest();
        }

        //Update Project
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectCreateDto projectUpdateDto)
        {
            if (ModelState.IsValid)
            {
                var projectDomain = new Project
                {
                    Id = id,
                    Name = projectUpdateDto.Name,
                    Description = projectUpdateDto.Description,
                    Deadline = projectUpdateDto.Deadline,
                    CreatedAt = projectUpdateDto.CreatedAt,
                    TeamLeaderId = projectUpdateDto.TeamLeaderId
                };
                return Ok(await _projectService.UpdateAsync(projectDomain));
            }
            return BadRequest();
        }

        //Delete Project
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _projectService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //get all projects with pagination and sorting
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _projectService.GetAllProjectsAsync(1, 10, "desc");
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //Get Project By Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _projectService.GetProjectById(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        //Assign Team Leader to Project and authorized to PM
        [HttpPost("assign-team-leader/{projectId:int}/{teamLeaderId}")]
        public async Task<IActionResult> AssignTeamLeader(int projectId, string teamLeaderId)
        {
            var result = await _projectService.AssignTeamLeaderAsync(projectId, teamLeaderId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //Add Team Members to Project and authorized to PM and TL on their respective projects
        [Authorize(Roles = "ProjectManager,TeamLeader")]
        [HttpPost("add-team-members/{projectId:int}")]
        public async Task<IActionResult> AddTeamMembers(int projectId, [FromBody] List<string> memberIds)
        {
            if (User.IsInRole("TeamLeader"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // check project ownership with projectId and userId
                var ownership = await _projectService.GetProjectsByTeamLeaderIdAsync(projectId, userId);
                if (!ownership.IsSuccess)
                {
                    return StatusCode(403, new ResponseDto { IsSuccess = false, ErrorMessage = "You dont own this course." });
                }
            }
            //if not TeamLeader then user is ProjectManager
            var result = await _projectService.AddTeamMembersAsync(projectId, memberIds);
            return Ok(result);

        }
    }
}
