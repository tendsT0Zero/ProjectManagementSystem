using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.API.Data;
using ProjectManagementSystem.API.Models;
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
            try
            {
                var histories = await _context.TaskSubmissions.Include(u => u.Task).ThenInclude(p => p.Project).Include(u => u.Submitter)
                                           .Where(ts => ts.Task.ProjectId == projectId && ts.TaskId == taskId)
                                           .OrderByDescending(ts => ts.SubmittedOn)
                                           .ToListAsync();
                if (histories == null || histories.Count == 0)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "No submission history found for the specified project and task."
                    };
                }
                var submissionHistories = histories.Select(h => new TaskSubmissionHistoryDto
                {
                    ProjectName = h.Task.Project.Name,
                    TaskTitle = h.Task.Title,
                    TaskDescription = h.Task.Description,
                    SubmissionDate = DateOnly.FromDateTime(h.SubmittedOn),
                    SubmitterName = h.Submitter.FullName,
                    SubmissionNotes = h.SubmissionNotes,
                    SubmissionAttachmentUrl = h.AttachmentUrl

                }).ToList();

                return new ResponseDto
                {
                    IsSuccess = true,
                    ResponseObject = submissionHistories
                };
            }catch(Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ResponseDto> PlaceASubmissionAsync(CreateTaskSubmissionDto dto, string userId)
        {
            try
            {
                string attachmentUrl = null;
                //check the user is reallly assigned to the task
                var task = await _context.Tasks.Include(t => t.AssignedToUser).FirstOrDefaultAsync(t => t.Id == dto.TaskId);
                if (task.AssignedToUser == null || task.AssignedToUser.Id != userId)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "You are not assigned to this task." };
                }
                //Now Proceed with file upload if any
                if (dto.File != null && dto.File.Length > 0)
                {

                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);


                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
                    var fullPath = Path.Combine(folderPath, fileName);


                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }


                    attachmentUrl = $"/uploads/{fileName}";
                }



                var submission = new TaskSubmission
                {
                    TaskId = dto.TaskId,
                    SubmissionNotes = dto.SubmissionNotes,
                    AttachmentUrl = attachmentUrl,
                    SubmitterId = userId,
                    SubmittedOn = DateTime.UtcNow
                };

                await _context.TaskSubmissions.AddAsync(submission);
                await _context.SaveChangesAsync();

                return new ResponseDto { IsSuccess = true, ErrorMessage = "Submitted Successfully" };
            }catch(Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }

        }
    }

    
}
