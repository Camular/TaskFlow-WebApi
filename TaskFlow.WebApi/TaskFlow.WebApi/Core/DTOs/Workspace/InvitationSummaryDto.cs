using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class InvitationSummaryDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Email { get; set; }
        public Guid RoleId { get; set; }
        public required string RoleName { get; set; }
        public required string Token { get; set; }
        public required InvitationStatus Status { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
