namespace TaskFlow.WebApi.Core.Entities
{
    public class Board
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required Workspace Workspace { get; set; }

    }
}
