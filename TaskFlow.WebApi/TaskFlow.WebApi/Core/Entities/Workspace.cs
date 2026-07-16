namespace TaskFlow.WebApi.Core.Entities
{
    public class Workspace
    {
        public Guid Id { get; set; }
        public required string SpaceName { get; set; }
        public string? SpaceDescription { get; set; }
        public bool IsPersonal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
        public ICollection<UserWorkspaceRole> UserWorkspaceRoles { get; set; } = new List<UserWorkspaceRole>();

    }
}
