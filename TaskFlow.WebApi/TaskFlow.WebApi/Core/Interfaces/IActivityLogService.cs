using TaskFlow.WebApi.Core.DTOs.Activity;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IActivityLogService
    {
        // Activity — list workspace activity, and write a log entry from other services.
        Task<List<ActivityLogSummaryDto>> GetWorkspaceActivityAsync(Guid userId, Guid workspaceId);
        Task LogAsync(Guid workspaceId, Guid actorUserId, string action, string message, Guid? taskItemId);
    }
}
