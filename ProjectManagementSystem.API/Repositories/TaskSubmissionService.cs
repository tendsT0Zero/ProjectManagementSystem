using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.API.Data;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public class TaskSubmissionService:ITaskSubmissionService
    {
        private readonly ApplicationDbContext _context;
        public TaskSubmissionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async  Task<ResponseDto> GetSubmissionHistoryAsync(int projectId, int taskId)
        {
            var history = await _context.TaskSubmissions.Include(u=>u.Task).Include(u=>u.Submitter)
                                                       .Where(ts => ts.Task.ProjectId == projectId && ts.TaskId == taskId)
                                                       .ToListAsync();
            if(history == null || history.Count == 0)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "No submission history found for the specified project and task."
                };
            }
            return new ResponseDto
            {
                IsSuccess = true,
                ResponseObject = history
            };
        }
    }
}
