using TaskStatus = TaskFlow.WebApi.Core.Entities.TaskStatus;

namespace TaskFlow.WebApi.Core.DTOs.Task
{
    public class TaskDetailDto
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required TaskStatus Status { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime? Deadline { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public bool IsOverdue => Deadline.HasValue
                          && Deadline.Value < DateTime.UtcNow
                          && Status != TaskStatus.Done;
    }
}
