using TaskFlow.WebApi.Core.DTOs.Task;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface ITaskItemService
    {
        Task<List<TaskSummaryDto>> GetUserTaskSummaryAsync(Guid userId); 
        Task<List<TaskSummaryDto>> GetWorkspaceTaskSummaryAsync(Guid userId, Guid workspaceId);
        Task<TaskSummaryDto> GetTaskByIdAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<TaskSummaryDto> CreateTaskAsync(Guid userId, Guid workspaceId, CreateTaskRequest request);
        Task<TaskSummaryDto> UpdateTaskAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskRequest request);
        Task<bool> DeleteTaskAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<TaskSummaryDto> AssignUserToTaskAsync(Guid requesterUserId, Guid workspaceId, Guid taskId, AssignUserToTaskRequest request);
        Task<TaskSummaryDto> UpdateTaskDeadlineAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskDeadlineRequest request);
        Task<TaskSummaryDto> UpdateTaskStatusAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskStatusRequest request);
    }
}
