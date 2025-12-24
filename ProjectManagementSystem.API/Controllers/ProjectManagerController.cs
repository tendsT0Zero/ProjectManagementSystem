using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Repositories;

namespace ProjectManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectManagerController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly ITaskService _taskService;
        public ProjectManagerController(IProjectService projectService, IProjectMemberService projectMemberService, ITaskService taskService)
        {
            _projectService = projectService;
            _projectMemberService = projectMemberService;
            _taskService = taskService;
        }


        //  assign team leader to project
        //Post: api/ProjectManager/{projectId}/assign-team-leader/{userId}
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

        // update project members
        // Put: api/ProjectManager/{projectId}/update-members
        [HttpPut("update-members/{projectId:int}")]
        public async Task<IActionResult> UpdateProjectMembers(int projectId, [FromBody] UpdateProjectMembersDto dto)
        {
            var result = await _projectMemberService.UpdateProjectMembersAsync(projectId, dto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // add new member to project
        // Post: api/ProjectManager/{projectId}/add-member/{memberId}
        [HttpPost("add-member/{projectId:int}/{memberId}")]
        public async Task<IActionResult> AddNewMemberToProject(int projectId, string memberId)
        {
            var result = await _projectMemberService.AddNewMemberToProjectAsync(projectId, memberId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // create task and assign to team member on a specific project
        // Post: api/ProjectManager/{projectId}/create-task/
        [HttpPost("create-task/{projectId:int}")]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] CreateTaskItemDto taskDto)
        {

            var taskItemdomain = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                CreatedAt = taskDto.CreatedAt,
                DueDate = taskDto.DueDate,
                ProjectId = projectId,
                AssignedToUserId = taskDto.AssignedToUserId
            };
            var result = await _taskService.CreateTaskAsync(projectId, taskItemdomain);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        // update task 
        // Put: api/ProjectManager/{projectId}/update-task/{taskId}
        [HttpPut("update-task/{projectId:int}/{taskId:int}")]
        public async Task<IActionResult> UpdateTask(int projectId, int taskId, [FromBody] UpdateTaskItemDto taskDto)
        {
            var taskItemdomain = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                DueDate = taskDto.DueDate,
                ProjectId = projectId,
                AssignedToUserId = taskDto.AssignedToUserId
            };
            var result = await _taskService.UpdateTaskAsync(taskId, taskItemdomain);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //get all the project details like ProjectName, TeamLeader'sName, MembersName
        //Get: api/ProjectManager/all-project-details
        [HttpGet("/all-project-details")]
        public async Task<IActionResult> GetAllProjectDetails()
        {
            var projectDetails = await _projectService.GetAllProjectDetailsAsync();
            if(projectDetails.IsSuccess)
            {
                return Ok(projectDetails.ResponseObject);
            }
            return BadRequest(projectDetails);
        }

        // get all tasks on a specific project
        // Get: api/ProjectManager/{projectId}/tasks

        [HttpGet("tasks/{projectId:int}")]
        public async Task<IActionResult> GetTasksByProjectId(int projectId)
        {
            var result = await _taskService.GetTasksByProjectIdAsync(projectId);
            if (result.ResponseObject!=null)
            {
               return Ok(result.ResponseObject);
            }
            return BadRequest();
        }
        // delete task
        // Delete: api/ProjectManager/delete-task/{taskId}
        [HttpDelete("delete-task/{taskId:int}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var result = await _taskService.DeleteTaskAsync(taskId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        //update user roles 
        // Put: api/ProjectManager/{projectId}/update-user-roles/
    }
}
