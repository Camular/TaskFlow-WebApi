using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Comment;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IActivityLogService _activityLogService;

        public CommentService(AppDbContext context, IActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        // ====================================================================================================
        // Guards
        // Shared workspace access check, task lookup, and comment lookup used by the actions below.
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

        private async Task<Comment> GetCommentOnTaskAsync(Guid taskId, Guid commentId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.TaskItemId == taskId && c.Id == commentId);

            if (comment == null)
            {
                throw new KeyNotFoundException("The comment was not found on this task.");
            }

            return comment;
        }


        // ====================================================================================================
        // Queries
        // List comments on a task.
        // ====================================================================================================

        public async Task<List<CommentSummaryDto>> GetTaskCommentsAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Comments.Read);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.TaskItemId == taskId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentSummaryDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    AuthorUserId = c.AuthorUserId,
                    CreatedAt = c.CreatedAt,
                    TaskItemId = c.TaskItemId,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

        }

        // ====================================================================================================
        // Commands
        // Create, update, and delete a comment on a task.
        // ====================================================================================================

        public async Task<CommentSummaryDto> CreateCommentAsync(Guid userId, Guid workspaceId, Guid taskId, CreateCommentRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Comments.Create);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                TaskItemId = taskId,
                AuthorUserId = userId,
                Body = request.Body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                TaskItem = null!,
                Author = null!
            };

            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            await _activityLogService.LogAsync(
                workspaceId,
                userId,
                "CommentCreated",
                "A comment was added to the task.",
                taskId);

            return new CommentSummaryDto
            {
                Id = newComment.Id,
                TaskItemId = newComment.TaskItemId,
                AuthorUserId = newComment.AuthorUserId,
                Body = newComment.Body,
                CreatedAt = newComment.CreatedAt,
                UpdatedAt = null
            };
        }

        public async Task<CommentSummaryDto> UpdateCommentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid commentId, UpdateCommentRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Comments.Update);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            var comment = await GetCommentOnTaskAsync(taskId, commentId);

            if (comment.AuthorUserId != userId)
            {
                throw new ForbiddenException("You can only edit your own comments.");
            }

            comment.Body = request.Body;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CommentSummaryDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                AuthorUserId = comment.AuthorUserId,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        public async Task<bool> DeleteCommentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid commentId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Comments.Delete);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            var comment = await GetCommentOnTaskAsync(taskId, commentId);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
