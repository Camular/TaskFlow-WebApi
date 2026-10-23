using TaskFlow.WebApi.Core.DTOs.Comment;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface ICommentService
    {
        // Comments — create a comment, list comments on a task, update a comment, and delete a comment.
        Task<List<CommentSummaryDto>> GetTaskCommentsAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<CommentSummaryDto> CreateCommentAsync(Guid userId, Guid workspaceId, Guid taskId, CreateCommentRequest request);
        Task<CommentSummaryDto> UpdateCommentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid commentId, UpdateCommentRequest request);
        Task<bool> DeleteCommentAsync(Guid userId, Guid workspaceId, Guid taskId, Guid commentId);
    }
}
