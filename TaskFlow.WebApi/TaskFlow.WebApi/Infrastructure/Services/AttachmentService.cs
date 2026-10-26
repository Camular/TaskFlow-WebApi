using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Attachment;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly AppDbContext _context;

        public AttachmentService(AppDbContext context)
        {
            _context = context;
        }

        // ====================================================================================================
        // Guards
        // Shared workspace access check, task lookup, and attachment lookup used by the actions below.
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

        private async Task<Attachment> GetAttachmentOnTaskAsync(Guid taskId, Guid attachmentId)
        {
            var attachment = await _context.Attachments
                .FirstOrDefaultAsync(a => a.TaskItemId == taskId && a.Id == attachmentId);

            if (attachment == null)
            {
                throw new KeyNotFoundException("The attachment was not found on this task.");
            }

            return attachment;
        }

        // ====================================================================================================
        // Queries
        // List attachments on a task.
        // ====================================================================================================

        public async Task<List<AttachmentSummaryDto>> GetTaskAttachmentsAsync(Guid userId, Guid workspaceId, Guid taskId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Attachments.Read);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            return await _context.Attachments
                .AsNoTracking()
                .Where(a => a.TaskItemId == taskId)
                .OrderBy(a => a.CreatedAt)
                .Select(a => new AttachmentSummaryDto
                {
                    Id = a.Id,
                    ContentType = a.ContentType,
                    OriginalFileName = a.OriginalFileName,
                    CreatedAt = a.CreatedAt,
                    SizeBytes = a.SizeBytes,
                    TaskItemId = a.TaskItemId,
                    UploadedByUserId = a.UploadedByUserId
                })
                .ToListAsync();

        }

        // ====================================================================================================
        // Commands
        // Upload a file to a task, and delete an attachment.
        // ====================================================================================================

        public async Task<AttachmentSummaryDto> UploadAttachmentAsync(Guid userId, Guid workspaceId, Guid taskId, IFormFile file)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Attachments.Create);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("A file is required.");
            }
                
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
  
            var extension = Path.GetExtension(file.FileName);

            string fileGuid = Guid.NewGuid().ToString();

            var storedFileName = fileGuid + extension;

            var fileAddress = Path.Combine(folderPath, storedFileName);

            using (var stream = new FileStream(fileAddress, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = file.ContentType,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                TaskItem = null!,
                UploadedBy = null!,
                CreatedAt = DateTime.UtcNow,
                SizeBytes = file.Length,
                TaskItemId = taskId,
                UploadedByUserId = userId

            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return new AttachmentSummaryDto
            {
                Id = attachment.Id,
                ContentType = attachment.ContentType,
                OriginalFileName = attachment.OriginalFileName,
                CreatedAt = attachment.CreatedAt,
                SizeBytes = attachment.SizeBytes,
                TaskItemId = attachment.TaskItemId,
                UploadedByUserId = attachment.UploadedByUserId
            };
        }
     public async Task<bool> DeleteAttachmentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid attachmentId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Attachments.Delete);

            await GetTaskInWorkspaceAsync(workspaceId, taskId);

            var attachment = await GetAttachmentOnTaskAsync(taskId, attachmentId);

            var fileAddress = Path.Combine(Directory.GetCurrentDirectory(), "uploads", attachment.StoredFileName);

            if (File.Exists(fileAddress))
            {
                File.Delete(fileAddress);
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
