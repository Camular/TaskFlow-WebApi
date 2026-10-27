using Microsoft.AspNetCore.Http;
using TaskFlow.WebApi.Core.DTOs.Attachment;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IAttachmentService
    {
        // Attachments — list and download attachments on a task, upload a file to a task, and delete an attachment.
        Task<List<AttachmentSummaryDto>> GetTaskAttachmentsAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<AttachmentFileDto> DownloadAttachmentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid attachmentId);
        Task<AttachmentSummaryDto> UploadAttachmentAsync(Guid userId, Guid workspaceId, Guid taskId, IFormFile file);
        Task<bool> DeleteAttachmentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid attachmentId);
    }
}
