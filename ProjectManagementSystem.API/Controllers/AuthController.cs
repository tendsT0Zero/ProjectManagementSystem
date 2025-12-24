using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Repositories;
using ProjectManagementSystem.API.Static_Details;

namespace ProjectManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        
        //[Authorize(Roles = "ProjectManager")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

       
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authService.LoginAsync(model);
            return Ok(token);
        }
    }
}
