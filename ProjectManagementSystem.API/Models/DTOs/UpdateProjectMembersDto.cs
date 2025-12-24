namespace ProjectManagementSystem.API.Models.DTOs
{
    public class UpdateProjectMembersDto
    {
        public List<string> MembersIds { get; set; } = new();
    }
}
