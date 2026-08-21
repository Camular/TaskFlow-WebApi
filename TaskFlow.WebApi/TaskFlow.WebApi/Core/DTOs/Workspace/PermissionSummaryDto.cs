namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class PermissionSummaryDto
    {
        public required Guid PermissionId { get; set; }
        public string? Code { get; set; }
    }
}
