using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public interface IProjectService
    {
        Task<ResponseDto> CreateAsync(Project project);
        Task<ResponseDto> UpdateAsync(Project project);
        Task<ResponseDto> DeleteAsync(int id);
        Task<ResponseDto> GetProjectById(int id);
        Task<ResponseDto> GetProjectsByTeamLeaderIdAsync(int projectId,string teamLeaderId);


        Task<ResponseDto> GetAllProjectsAsync(int pageNumber, int pageSize, string sort);

        Task<ResponseDto> AssignTeamLeaderAsync(int projectId, string teamLeaderId);
        Task<ResponseDto> AddTeamMembersAsync(int projectId, List<string> memberIds);

        Task<ResponseDto> GetAllProjectDetailsAsync();
    }
}