using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystem.API.Data;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;
using ProjectManagementSystem.API.Static_Details;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementSystem.API.Repositories
{
    public class AuthService:IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext context,UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            _context = context; 
            _userManager = userManager;
            _configuration = configuration;
        }

        #region Generate JWT Token
        public async Task<string> GenerateJwtTokens(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JWTSetting");

            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion

        #region Register New User

        public async Task<string> RegisterAsync(UserRegistrationRequestDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return "User Already Exist with the Same Email.";

            
            if (dto.Role != SD.UserRoleType.TeamLeader.ToString() &&
                dto.Role != SD.UserRoleType.TeamMember.ToString())
            {
                return "Invalid role assignment. Only TeamLeader or TeamMember are allowed.";
            }

            var newUser = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var isCreated = await _userManager.CreateAsync(newUser, dto.Password);

            if (!isCreated.Succeeded)
                return $"Registration Failed: {string.Join(", ", isCreated.Errors.Select(e => e.Description))}";

            var roleResult = await _userManager.AddToRoleAsync(newUser, dto.Role);

            if (!roleResult.Succeeded)
                return $"User created but failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}";

            return "User Registered Successfully.";
        }


        #endregion

        #region User Login
        public async Task<string> LoginAsync(LoginRequestDto dto)
        {
            // check for username
            var existingUser=await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && await _userManager.CheckPasswordAsync(existingUser,dto.Password)) 
            {
                return await GenerateJwtTokens(existingUser);
            }

            return "Incorrect Login Credentials. Please Try Again.";
        }
        #endregion
    }
}
