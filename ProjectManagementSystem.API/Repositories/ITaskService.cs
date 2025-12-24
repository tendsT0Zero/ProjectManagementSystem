using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public interface ITaskService
    {
        // create task and assign to team member on a specific project
        // Post: api/ProjectManager/{projectId}/create-task/
        Task<ResponseDto> CreateTaskAsync(int projectId, TaskItem task);

        Task<ResponseDto> UpdateTaskAsync(int id, TaskItem task);

        Task<ResponseDto> GetTasksByProjectIdAsync(int projectId);
    }
}
