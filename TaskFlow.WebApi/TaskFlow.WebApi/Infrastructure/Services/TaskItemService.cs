using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Task;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;
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

        #region Projections & Mapping Helpers

        private static readonly Expression<Func<TaskItem, TaskSummaryDto>> AsTaskSummaryDto = t => new TaskSummaryDto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            Deadline = t.Deadline,
            AssignedUserId = t.AssignedUserId,
            WorkspaceId = t.WorkspaceId
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
            WorkspaceId = t.WorkspaceId
        };
        private static TaskSummaryDto MapToTaskSummaryDto(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            Deadline = task.Deadline,
            AssignedUserId = task.AssignedUserId,
            WorkspaceId = task.WorkspaceId
        };

        #endregion

        #region Helper Methods

        private async Task CheckWorkspacePermissionAsync(Guid userId, Guid workspaceId, string permissionCode)
        {
            var isMember = await _context.UserWorkspaceRoles
                .AsNoTracking()
                .AnyAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new KeyNotFoundException("Çalışma alanı bulunamadı veya bu çalışma alanına erişim izniniz yok.");
            }

            var hasPermission = await _context.UserWorkspaceRoles
                .AsNoTracking()
                .AnyAsync(uwr => uwr.UserId == userId &&
                                 uwr.WorkspaceId == workspaceId &&
                                 uwr.Role.RolePermissions.Any(rp => rp.Permission.Code == permissionCode));

            if (!hasPermission)
            {
                throw new ForbiddenException($"Bu işlemi gerçekleştirmek için '{permissionCode}' yetkiniz bulunmamaktadır.");
            }
        }

        private async Task<TaskItem> GetTaskInWorkspaceAsync(Guid workspaceId, Guid taskId)
        {
            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.WorkspaceId == workspaceId && t.Id == taskId);

            if (taskItem == null)
            {
                throw new KeyNotFoundException("İşlem yapmak istediğiniz Task bulunamadı ya da bulunduğunuz çalışma alanında yer almıyor.");
            }

            return taskItem;
        }

        #endregion

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
                throw new KeyNotFoundException("Görüntülemek istediğiniz Task bulunamadı ya da bulunduğunuz çalışma alanında bulunmamakta.");
            }

            return taskSummary;
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
                throw new KeyNotFoundException("Görüntülemek istediğiniz Task bulunamadı ya da bulunduğunuz çalışma alanında bulunmamakta.");
            }

            return taskSummary;
        }

        public async Task<TaskSummaryDto> CreateTaskAsync(Guid userId, Guid workspaceId, CreateTaskRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Create);

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
                AssignedUser = null!
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);
        }

        public async Task<TaskSummaryDto> UpdateTaskAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            taskItem.Title = request.Title;
            taskItem.Description = request.Description;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);
        }

        public async Task<bool> DeleteTaskAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Delete);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return true;
        }

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
                throw new KeyNotFoundException("Eklenmek istenen e-posta adresine sahip bir kullanıcı bulunamadı.");
            }

            var isMember = await _context.UserWorkspaceRoles
                .AsNoTracking()
                .AnyAsync(t => t.UserId == targetUser.Id && t.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new InvalidOperationException("Atamak istediğiniz kişi bu çalışma alanında bulunmamaktadır.");
            }

            taskItem.AssignedUserId = targetUser.Id;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);
        }

        public async Task<TaskSummaryDto> UpdateTaskDeadlineAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskDeadlineRequest request)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.DeadlineUpdate);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            if (taskItem.Status == TaskStatus.Done || taskItem.Status == TaskStatus.Failed)
            {
                throw new InvalidOperationException("Tamamlanmış veya iptal edilmiş görevlerin son teslim tarihi değiştirilemez.");
            }

            if (request.Deadline.HasValue && request.Deadline.Value < taskItem.CreatedAt)
            {
                throw new InvalidOperationException("Son teslim tarihi, görevin oluşturulma tarihinden daha eski bir tarih olamaz.");
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

        public async Task<TaskSummaryDto> UnassignFromTaskAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckWorkspacePermissionAsync(userId, workspaceId, SystemPermissions.TaskItem.Assign);

            var taskItem = await GetTaskInWorkspaceAsync(workspaceId, taskId);

            taskItem.AssignedUserId = null;

            await _context.SaveChangesAsync();

            return MapToTaskSummaryDto(taskItem);

        }

    }
}