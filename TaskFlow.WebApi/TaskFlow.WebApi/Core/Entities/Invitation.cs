namespace TaskFlow.WebApi.Core.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Email { get; set; }
        public Guid RoleId { get; set; }
        public Guid InvitedByUserId { get; set; }
        public required string Token { get; set; }
        public required InvitationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public required User InvitedBy { get; set; }
        public required WorkspaceRole Role { get; set; }
        public required Workspace Workspace { get; set; }
    }
}
