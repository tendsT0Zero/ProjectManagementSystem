using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.API.Data;
using ProjectManagementSystem.API.Models;
using ProjectManagementSystem.API.Models.DTOs;

namespace ProjectManagementSystem.API.Repositories
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ResponseDto> CreateTaskAsync(int projectId, TaskItem task)
        {
            try
            {
                //first check if the project exists
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Project not found." };
                }
                //check if the assigned user exists
                var assignedUser = await _userManager.FindByIdAsync(task.AssignedToUserId);
                if (assignedUser == null)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Assigned user not found." };
                }
                //check if the assigned user is a teamleader or member of the project
                var isTeamLeader = project.TeamLeaderId == assignedUser.Id;
                var isTeamMember = await _context.Members.AnyAsync(m => m.ProjectId == projectId && m.UserId == assignedUser.Id);
                if (!isTeamLeader && !isTeamMember)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Assigned user is not a member of the project." };
                }
                //assign the project to the task
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                return new ResponseDto { IsSuccess = true, ResponseObject = task, ErrorMessage = "Created successfully!" };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                };
            }
        }

        public async Task<ResponseDto> UpdateTaskAsync(int id, TaskItem task)
        {
            try
            {
                //first check if the task exists with the projectid
                var existingTask = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
                if (existingTask == null || existingTask.ProjectId != task.ProjectId)
                {
                    return new ResponseDto { IsSuccess = false, ErrorMessage = "Task not found related to that project." };

                }
                // now update the task
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.ProjectId = task.ProjectId;
                existingTask.AssignedToUserId = task.AssignedToUserId;
                existingTask.Status = task.Status;
                existingTask.Priority = task.Priority;
                existingTask.DueDate = task.DueDate;
                existingTask.IsRequiredAttachment = task.IsRequiredAttachment;

                await _context.SaveChangesAsync();
                return new ResponseDto { IsSuccess = true, ResponseObject = existingTask, ErrorMessage = "Updated successfully!" };
            }
            catch(Exception ex) {
            
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
            }
        }

        public async Task<ResponseDto> GetTasksByProjectIdAsync(int projectId)
        {
            try
            {
                var tasks = await _context.Tasks.Include(u => u.Project).Include(a => a.AssignedToUser).Where(u => u.ProjectId == projectId).ToListAsync();
                if (tasks == null || tasks.Count <= 0)
                {
                    return new ResponseDto
                    {
                        IsSuccess = true,
                        ResponseObject = null
                    };
                }
                var tasksdetails = tasks.Select(task => new TaskDetailsDto
                {
                    ProjectName = task.Project.Name,
                    Title = task.Title,
                    Description = task.Description != null ? task.Description : "No Description Added on this Task.",
                    AssignedToUserName = task.AssignedToUser.FullName,
                    DueDate = (DateTime)(task.DueDate != null ? task.DueDate : task.Project.Deadline),
                    Status = task.Status,
                    IsRequiredAttachment = task.IsRequiredAttachment

                }).ToList();

                return new ResponseDto
                {
                    ResponseObject = tasksdetails,
                    IsSuccess = true
                };
            }catch(Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess= false,
                    ErrorMessage=ex.Message
                };
            }
        }

        public async Task<ResponseDto> DeleteTaskAsync(int id)
        {
            try
            {
                //check really the any task exist with the id.?
                var existingTask = await _context.Tasks.FirstOrDefaultAsync(u => u.Id == id);
                if (existingTask != null)
                {
                    _context.Tasks.Remove(existingTask);
                    await _context.SaveChangesAsync();
                    return new ResponseDto
                    {
                        IsSuccess = true,
                        ResponseObject = existingTask
                    };
                }

                return new ResponseDto
                {
                    IsSuccess = false,
                    ResponseObject = existingTask,
                    ErrorMessage = "Failed to delete"
                };
            }catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ResponseDto?> GetTasksByAssignedUserIdAsync(string? userId)
        {
            try
            {
                var tasks = await _context.Tasks.Include(u => u.Project).Where(u => u.AssignedToUserId == userId).ToListAsync();
                if (tasks.Any())
                {
                    var tasksList = new List<TaskItemResponseDto>();
                    tasksList = tasks.Select(u => new TaskItemResponseDto
                    {
                        Title = u.Title,
                        Description = u.Description,
                        ProjectName = u.Project.Name,
                        DueDate = u.DueDate,
                        Status = u.Status,
                        IsRequiredAttachment = u.IsRequiredAttachment,


                    }).ToList();

                    return new ResponseDto
                    {
                        ResponseObject = tasksList,
                        IsSuccess = true
                    };
                }
                return new ResponseDto
                {
                    ResponseObject = null,
                    IsSuccess = false,
                    ErrorMessage = "User Not Found."
                };
            }catch(Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess=false,
                    ErrorMessage = ex.Message
                };
            }
        }

    }
}
