namespace TaskFlow.WebApi.Core.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid ActorUserId { get; set; }
        public Guid? TaskItemId { get; set; }
        public required string Action { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required Workspace Workspace { get; set; }
        public required User Actor { get; set; }
        public TaskItem? TaskItem { get; set; }

    }
}
