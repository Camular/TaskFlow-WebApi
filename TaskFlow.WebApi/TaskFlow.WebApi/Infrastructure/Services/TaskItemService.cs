using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Task;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;
using static TaskFlow.WebApi.Core.Authorization.SystemPermissions;
using TaskItem = TaskFlow.WebApi.Core.Entities.TaskItem;
using TaskStatus = TaskFlow.WebApi.Core.Entities.TaskStatus;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly AppDbContext _context;

        public TaskItemService(AppDbContext context)
        {
            _context = context;
        }

        // ====================================================================================================
        // Mapping
        // Maps a task row to a summary or detail response.
        // ====================================================================================================

        private static readonly Expression<Func<TaskItem, TaskSummaryDto>> AsTaskSummaryDto = t => new TaskSummaryDto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            Deadline = t.Deadline,
            AssignedUserId = t.AssignedUserId,
            WorkspaceId = t.WorkspaceId,
            ProjectId = t.ProjectId,
            Number = t.Number,
            Code = (t.Number != null && t.Project != null && t.Project.Key != null) ? t.Project.Key + "-" + t.Number : null
        };

        private static readonly Expression<Func<TaskItem, TaskDetailDto>> AsTaskDetailDto = t => new TaskDetailDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            Deadline = t.Deadline,
            AssignedUserId = t.AssignedUserId,
            WorkspaceId = t.WorkspaceId,
            ProjectId = t.ProjectId,
            Number = t.Number,
            Code = (t.Number != null && t.Project != null && t.Project.Key != null) ? t.Project.Key + "-" + t.Number : null
        };
        private static TaskSummaryDto MapToTaskSummaryDto(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            Deadline = task.Deadline,
            AssignedUserId = task.AssignedUserId,
            WorkspaceId = task.WorkspaceId,
            ProjectId = task.ProjectId,
            Number = task.Number,
            Code = (task.Number != null && task.Project != null && task.Project.Key != null) ? task.Project.Key + "-" + task.Number : null
        };

        // ====================================================================================================
        // Guards
        // Shared workspace access check and task lookup used by the actions below.
        // ====================================================================================================

        private async Task CheckWorkspacePermissionAsync(Guid userId, Guid workspaceId, string permissionCode)
        {
            var isMember = await _context.UserWorkspaceRoles
                .AsNoTracking()
                .AnyAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new KeyNotFoundException("Workspace was not found, or you do not have access to it.");
            }

            var hasPermission = await _context.UserWorkspaceRoles
                .AsNoTracking()
                .AnyAsync(uwr => uwr.UserId == userId &&
                                 uwr.WorkspaceId == workspaceId &&
                                 uwr.Role.RolePermissions.Any(rp => rp.Permission.Code == permissionCode));

            if (!hasPermission)
            {
                throw new ForbiddenException($"You do not have the '{permissionCode}' permission required for this operation.");
            }
        }

        private async Task<TaskItem> GetTaskInWorkspaceAsync(Guid workspaceId, Guid taskId)
        {
            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.WorkspaceId == workspaceId && t.Id == taskId);

            if (taskItem == null)
            {
                throw new KeyNotFoundException("The task was not found in this workspace.");
            }

            return taskItem;
        }


        // ====================================================================================================
        // Queries
        // Read task lists, a task summary, and a task detail.
        // ====================================================================================================

        public async Task<List<TaskSummaryDto>> GetUserTaskSummaryAsync(Guid userId)
        {
            
            return await _context.TaskItems
                .AsNoTracking()
                .Where(t => t.AssignedUserId == userId)
                .Select(AsTaskSummaryDto)
                .ToListAsync();
        }

        public async Task<List<TaskSummaryDto>> GetWorkspaceTaskSummaryAsync(Guid userId, Guid workspaceId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Read);

            return await _context.TaskItems
                .AsNoTracking()
                .Where(t => t.WorkspaceId == workspaceId)
                .Select(AsTaskSummaryDto)
                .ToListAsync();
        }

        public async Task<TaskSummaryDto> GetTaskByIdAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Read);

            var taskSummary = await _context.TaskItems
                .AsNoTracking()
                .Where(t => t.WorkspaceId == workspaceId && t.Id == taskId)
                .Select(AsTaskSummaryDto)
                .FirstOrDefaultAsync();

            if (taskSummary == null)
            {
                throw new KeyNotFoundException("The task was not found in this workspace.");
            }

            return taskSummary;
        }

        public async Task<TaskDetailDto> GetTaskDetailAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Read);

            var taskSummary = await _context.TaskItems
                .AsNoTracking()
                .Where(t => t.WorkspaceId == workspaceId && t.Id == taskId)
                .Select(AsTaskDetailDto)
                .FirstOrDefaultAsync();

            if (taskSummary == null)
            {
                throw new KeyNotFoundException("The task was not found in this workspace.");
            }

            return taskSummary;
        }


        // ====================================================================================================
        // Commands
        // Create, update, and delete a task.
        // ====================================================================================================

        public async Task<TaskSummaryDto> CreateTaskAsync(Guid userId, Guid workspaceId, CreateTaskRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Create);

            Project? project = null;

            int? taskNumber = null;

            if (request.ProjectId != null)
            {
                project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.WorkspaceId == workspaceId);

                if (project == null)
                    throw new KeyNotFoundException("The project was not found in this workspace.");

                if (project.Key == null)
                    throw new InvalidOperationException("This project does not have a key.");

                var number = await _context.TaskItems
                    .Where(t => t.ProjectId == request.ProjectId)
                    .OrderByDescending(t => t.Number)
                    .Select(t => t.Number)
                    .FirstOrDefaultAsync();

                taskNumber = (number ?? 0) + 1;
            }

            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                Deadline = request.Deadline.HasValue
                    ? DateTime.SpecifyKind(request.Deadline.Value, DateTimeKind.Utc)
                    : null,
                Status = TaskStatus.ToDo,
                WorkspaceId = workspaceId,
                AssignedUserId = null,
                Workspace = null!,
                AssignedUser = null!,
                ProjectId = request.ProjectId,
                Project = null,
                Number = taskNumber
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            var dto = MapToTaskSummaryDto(taskItem);

            if (project != null && taskNumber != null)
            {
                dto.Code = project.Key + "-" + taskNumber;
            }

            return dto;
        }

        public async Task<TaskSummaryDto> UpdateTaskAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            Project? project = null;

            if (request.ProjectId != taskItem.ProjectId)
            {
                if (request.ProjectId == null)
                {
                    taskItem.Number = null;
                }
                else
                {
                    project = await _context.Projects
                        .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.WorkspaceId == workspaceId);

                    if (project == null)
                        throw new KeyNotFoundException("The project was not found in this workspace.");

                    if (project.Key == null)
                        throw new InvalidOperationException("This project does not have a key.");

                    var number = await _context.TaskItems
                        .Where(t => t.ProjectId == request.ProjectId)
                        .OrderByDescending(t => t.Number)
                        .Select(t => t.Number)
                        .FirstOrDefaultAsync();

                    taskItem.Number = (number ?? 0) + 1;
                }
            }
            else
            {
                if (taskItem.ProjectId != null)
                {
                    project = await _context.Projects
                        .FirstOrDefaultAsync(p => p.Id == taskItem.ProjectId && p.WorkspaceId == workspaceId);
                }
            }

            taskItem.Title = request.Title;
            taskItem.Description = request.Description;
            taskItem.ProjectId = request.ProjectId;

            await _context.SaveChangesAsync();

            var dto = MapToTaskSummaryDto(taskItem);

            if (taskItem.Number != null && project?.Key != null)
            {
                dto.Code = project.Key + "-" + taskItem.Number;
            }

            return dto;
        }

        public async Task<bool> DeleteTaskAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Delete);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return true;
        }


        // ====================================================================================================
        // Assignment and schedule
        // Assign or clear the assignee, and change the deadline or status.
        // ====================================================================================================

        public async Task<TaskSummaryDto> AssignUserToTaskAsync(Guid requesterUserId, Guid workspaceId, Guid taskId, AssignUserToTaskRequest request)
        {
            await CheckWorkspacePermissionAsync(requesterUserId, workspaceId, SystemPermissions.TaskItem.Assign);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            var normalizedEmail = request.Email.ToLowerInvariant();

            var targetUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (targetUser == null)
            {
                throw new KeyNotFoundException("No user was found for the supplied email address.");
            }

            var isMember = await _context.UserWorkspaceRoles
                .AsNoTracking()
                .AnyAsync(t => t.UserId == targetUser.Id && t.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new InvalidOperationException("The user you want to assign is not a member of this workspace.");
            }

            taskItem.AssignedUserId = targetUser.Id;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);
        }

        public async Task<TaskSummaryDto> UnassignFromTaskAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Assign);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            taskItem.AssignedUserId = null;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);

        }

        public async Task<TaskSummaryDto> UpdateTaskDeadlineAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskDeadlineRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.DeadlineUpdate);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            if (taskItem.Status == TaskStatus.Done || taskItem.Status == TaskStatus.Failed)
            {
                throw new InvalidOperationException("The deadline of a completed or failed task cannot be changed.");
            }

            if (request.Deadline.HasValue && request.Deadline.Value < taskItem.CreatedAt)
            {
                throw new InvalidOperationException("The deadline cannot be earlier than the date the task was created.");
            }

            taskItem.Deadline = request.Deadline.HasValue
                ? DateTime.SpecifyKind(request.Deadline.Value, DateTimeKind.Utc)
                : null;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);
        }

        public async Task<TaskSummaryDto> UpdateTaskStatusAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskStatusRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.StatusUpdate);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            taskItem.Status = request.Status;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);
        }

    }
}