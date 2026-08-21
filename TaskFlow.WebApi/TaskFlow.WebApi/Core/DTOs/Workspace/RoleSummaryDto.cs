namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class RoleSummaryDto
    {
        public required Guid RoleId { get; set; }
        public Guid? WorkspaceId { get; set; }
        public required string Name { get; set; }
        public List<AssignedUsersSummaryDto>? AssignedUsers { get; set; } = new List<AssignedUsersSummaryDto>();
        public required List<PermissionSummaryDto> Permissions { get; set; } = new List<PermissionSummaryDto>();
    }
}
