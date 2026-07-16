namespace TaskFlow.WebApi.Core.Entities
{
    public class WorkspaceRole
    {
        public Guid Id { get; set; }
        public Guid? WorkspaceId { get; set; }
        public required string Name { get; set; }
        public Workspace? Workspace { get; set; }
        public ICollection<UserWorkspaceRole> UserWorkspaceRoles { get; set; } = new List<UserWorkspaceRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
