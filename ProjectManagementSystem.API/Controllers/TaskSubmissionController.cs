using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Repositories;

namespace ProjectManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskSubmissionController : ControllerBase
    {
        private readonly ITaskSubmissionService _taskSubmissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITaskService _taskService;
        public TaskSubmissionController(ITaskSubmissionService taskSubmissionService, UserManager<ApplicationUser> userManager, ITaskService taskService)
        {
            _taskSubmissionService = taskSubmissionService;
            _userManager = userManager;
            _taskService = taskService;
        }

        //get all the task of authorized user
        [HttpGet("my-tasks")]
        [Authorize(Roles = "TeamMember,TeamLeader")]
        public async Task<IActionResult> GetMyTaskSubmissions()
        {
            var userId = _userManager.GetUserId(User);
            var result = await _taskService.GetTasksByAssignedUserIdAsync(userId);
            return Ok(result);
        }

        //submit task
        [HttpPost("submit-task")]
        [Authorize(Roles = "TeamMember,TeamLeader")]
         public async Task<IActionResult> SubmitTask([FromForm] CreateTaskSubmissionDto taskSubmissionDto)
        {
            var userId = _userManager.GetUserId(User);
            return Ok(await _taskSubmissionService.PlaceASubmissionAsync(taskSubmissionDto, userId));
        }
    }
}
