namespace TaskFlow.WebApi.Core.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required Workspace Workspace { get; set; }

    }
}
