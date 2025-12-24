namespace ProjectManagementSystem.API.Models.DTOs
{
    public class ProjectDetailsDto
    {
        public string ProjectName { get; set; } = string.Empty;
        public string TeamLeaderName { get; set; } = string.Empty;
        public List<string> MemberNames { get; set; } = new();
    }
}
