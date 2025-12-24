using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokens(ApplicationUser user);
        Task<string> RegisterAsync(UserRegistrationRequestDto dto);
        Task<string>LoginAsync(LoginRequestDto dto);
    }
}
