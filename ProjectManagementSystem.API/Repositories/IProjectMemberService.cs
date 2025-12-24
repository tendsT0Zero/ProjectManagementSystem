using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public interface IProjectMemberService
    {
        Task<ResponseDto> UpdateProjectMembersAsync(int projectId, UpdateProjectMembersDto updateProjectMembersDto);
        Task<ResponseDto> AddNewMemberToProjectAsync(int projectId, string memberId);
    }
}
