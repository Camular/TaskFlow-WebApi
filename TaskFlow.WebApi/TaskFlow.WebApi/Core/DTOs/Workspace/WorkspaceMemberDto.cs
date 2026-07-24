namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class WorkspaceMemberDto
    {
        public required Guid UserId { get; set; }
        public required Guid RoleId { get; set; }
        public required string UserName { get; set; }
        public required string RoleName { get; set; }
        public required string Email { get; set; }
    }
}
