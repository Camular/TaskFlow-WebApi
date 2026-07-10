namespace TaskFlow.WebApi.Core.Entities
{
    public class UserWorkspaceRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public Guid WorkspaceId { get; set; }
        public required User User { get; set; }
        public required WorkspaceRole Role { get; set; }
        public required Workspace Workspace { get; set; }
    }
}
