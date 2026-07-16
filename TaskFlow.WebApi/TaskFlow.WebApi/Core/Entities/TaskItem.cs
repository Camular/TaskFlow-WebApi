namespace TaskFlow.WebApi.Core.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public required Workspace Workspace { get; set; }
        public User? AssignedUser { get; set; }
        
    }
}
