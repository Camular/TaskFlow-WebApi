using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Activity;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly AppDbContext _context;

        public ActivityLogService(AppDbContext context)
        {
            _context = context;
        }

        // ====================================================================================================
        // Guards
        // Shared workspace access check used by the actions below.
        // ====================================================================================================

        private async Task<UserWorkspaceRole> CheckAndGetWorkspaceRoleAsync(Guid userId, Guid workspaceId, string permissionCode)
        {
            var userWorkspaceRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.Workspace)
                .Include(uwr => uwr.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (userWorkspaceRole == null)
            {
                throw new KeyNotFoundException("Workspace was not found, or you do not have access to it.");
            }

            var hasPermission = userWorkspaceRole.Role.RolePermissions
                .Any(rp => rp.Permission.Code == permissionCode);

            if (!hasPermission)
            {
                throw new ForbiddenException($"You do not have the '{permissionCode}' permission required for this operation.");
            }

            return userWorkspaceRole;
        }

        // ====================================================================================================
        // Queries
        // List activity in a workspace.
        // ====================================================================================================

        public async Task<List<ActivityLogSummaryDto>> GetWorkspaceActivityAsync(Guid userId, Guid workspaceId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.ActivityLog.Read);

            return await _context.ActivityLogs
                .AsNoTracking()
                .Where(a => a.WorkspaceId == workspaceId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ActivityLogSummaryDto
                {
                    Id = a.Id,
                    Action = a.Action,
                    Message = a.Message,
                    CreatedAt = a.CreatedAt,
                    WorkspaceId = a.WorkspaceId,
                    ActorUserId = a.ActorUserId,
                    TaskItemId = a.TaskItemId
                })
                .ToListAsync();
        }

        // ====================================================================================================
        // Commands
        // Write an activity log entry (called from other services).
        // ====================================================================================================

        public async Task LogAsync(Guid workspaceId, Guid actorUserId, string action, string message, Guid? taskItemId)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException("Activity action is required.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Activity message is required.");
            }

            var activityLog = new ActivityLog
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                ActorUserId = actorUserId,
                TaskItemId = taskItemId,
                Action = action,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Workspace = null!,
                Actor = null!,
                TaskItem = null
            };

            await _context.ActivityLogs.AddAsync(activityLog);
            await _context.SaveChangesAsync();
        }
    }
}
