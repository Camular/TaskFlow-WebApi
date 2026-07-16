namespace TaskFlow.WebApi.Core.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public WorkspaceRole Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;

    }
}
