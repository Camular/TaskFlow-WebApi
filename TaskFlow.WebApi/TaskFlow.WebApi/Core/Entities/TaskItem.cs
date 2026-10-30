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
        public Guid? ProjectId { get; set; }
        public int? Number { get; set; }
        public Project? Project { get; set; }
        public Guid? ColumnId { get; set; }
        public Column? Column { get; set; }
        public Guid? AssignedUserId { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public required Workspace Workspace { get; set; }
        public User? AssignedUser { get; set; }
        public User? CreatedByUser { get; set; }

    }
}
