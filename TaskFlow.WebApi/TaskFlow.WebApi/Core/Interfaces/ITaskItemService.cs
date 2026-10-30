using TaskFlow.WebApi.Core.DTOs.Task;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface ITaskItemService
    {
        // Queries — read task lists, a task summary, a task detail, and get a task by code.
        Task<List<TaskSummaryDto>> GetUserTaskSummaryAsync(Guid userId);
        Task<List<TaskSummaryDto>> GetWorkspaceTaskSummaryAsync(Guid userId, Guid workspaceId);
        Task<TaskSummaryDto> GetTaskByIdAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<TaskDetailDto> GetTaskDetailAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<TaskSummaryDto> GetTaskByCodeAsync(Guid userId, Guid workspaceId, string code);

        // Commands — create, update, and delete a task.
        Task<TaskSummaryDto> CreateTaskAsync(Guid userId, Guid workspaceId, CreateTaskRequest request);
        Task<TaskSummaryDto> UpdateTaskAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskRequest request);
        Task<bool> DeleteTaskAsync(Guid userId, Guid workspaceId, Guid taskId);

        // Assignment and schedule — assign or clear the assignee, change the deadline or status, and move a task to a column.
        Task<TaskSummaryDto> AssignUserToTaskAsync(Guid requesterUserId, Guid workspaceId, Guid taskId, AssignUserToTaskRequest request);
        Task<TaskSummaryDto> UnassignFromTaskAsync(Guid userId, Guid workspaceId, Guid taskId);
        Task<TaskSummaryDto> UpdateTaskDeadlineAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskDeadlineRequest request);
        Task<TaskSummaryDto> UpdateTaskStatusAsync(Guid userId, Guid workspaceId, Guid taskId, UpdateTaskStatusRequest request);
        Task<TaskSummaryDto> MoveTaskToColumnAsync(Guid userId, Guid workspaceId, Guid taskId, MoveTaskToColumnRequest request);
    }
}
