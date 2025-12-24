using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Repositories;

namespace ProjectManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        //Create Project
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProjectCreateDto projectDto)
        {
            return Ok();
        }

        //Update Project


        //Delete Project


        //get all projects with pagination and sorting


        //Get Project By Id

        //Assign Team Leader to Project and authorized to PM


        //Add Team Members to Project and authorized to PM and TL on their respective projects
    }
}
