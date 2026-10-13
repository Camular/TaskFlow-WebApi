using TaskFlow.WebApi.Core.DTOs.Workspace;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IWorkspaceService
    {
        // Workspaces — list, get, create, update, and delete a workspace.
        Task<List<WorkspaceSummaryDto>> GetUserWorkspaceSummaryAsync(Guid userId);
        Task<WorkspaceSummaryDto> GetWorkspaceByIdAsync(Guid userId, Guid workspaceId);
        Task<WorkspaceSummaryDto> CreateWorkspaceAsync(Guid userId, CreateWorkspaceRequest request);
        Task<WorkspaceSummaryDto> UpdateWorkspaceAsync(Guid userId, Guid workspaceId, UpdateWorkspaceRequest request);
        Task<bool> DeleteWorkspaceAsync(Guid userId, Guid workspaceId);

        // Members — list members, change a member role, and remove a member.
        Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(Guid userId, Guid workspaceId);
        Task<WorkspaceMemberDto> UpdateMemberRoleAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId, UpdateMemberRoleRequest request);
        Task<bool> RemoveWorkspaceMemberAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId);

        // Roles — list, get, create, update, and delete custom workspace roles.
        Task<List<RoleSummaryDto>> GetWorkspaceRolesSummaryAsync(Guid userId, Guid workspaceId);
        Task<RoleSummaryDto> GetWorkspaceRoleByIdSummaryAsync(Guid userId, Guid workspaceId, Guid roleId);
        Task<RoleSummaryDto> CreateWorkspaceRoleAsync(Guid userId, Guid workspaceId, CreateWorkspaceRoleRequest request);
        Task<RoleSummaryDto> UpdateWorkspaceRoleAsync(Guid userId, Guid workspaceId, Guid roleId, UpdateWorkspaceRoleRequest request);
        Task<bool> DeleteWorkspaceRoleAsync(Guid userId, Guid workspaceId, Guid roleId);

        // Invitations — list, create, cancel, and accept workspace invitations.
        Task<List<InvitationSummaryDto>> GetWorkspaceInvitationsAsync(Guid userId, Guid workspaceId);
        Task<InvitationSummaryDto> CreateInvitationAsync(Guid userId, Guid workspaceId, CreateInvitationRequest request);
        Task<bool> CancelWorkspaceInvitationAsync(Guid userId, Guid workspaceId, Guid invitationId);
        Task<WorkspaceMemberDto> AcceptInvitationAsync(Guid userId, AcceptInvitationRequest request);
    }
}
