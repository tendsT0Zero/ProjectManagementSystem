using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public interface ITaskSubmissionService
    {
        Task<ResponseDto> GetSubmissionHistoryAsync(int projectId,int taskId);
        Task<ResponseDto> PlaceASubmissionAsync(TaskSubmission taskSubmission, string userId);
    }
}
