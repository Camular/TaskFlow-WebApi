using TaskFlow.WebApi.Core.DTOs.Workspace;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IWorkspaceService
    {
        Task<List<WorkspaceSummaryDto>> GetUserWorkspaceSummaryAsync(Guid userId);
        Task<WorkspaceSummaryDto> CreateWorkspaceAsync(Guid userId, CreateWorkspaceRequest request);
        Task<WorkspaceSummaryDto> UpdateWorkspaceAsync(Guid userId, Guid workspaceId, UpdateWorkspaceRequest request);
        Task<bool> DeleteWorkspaceAsync(Guid userId, Guid workspaceId);
        Task<WorkspaceSummaryDto> GetWorkspaceByIdAsync(Guid userId, Guid workspaceId);
        Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(Guid userId, Guid workspaceId);
        Task<WorkspaceMemberDto> AddWorkspaceMemberAsync(Guid requesterUserId, Guid workspaceId, AddWorkspaceMemberRequest request);
        Task<WorkspaceMemberDto> UpdateMemberRoleAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId, UpdateMemberRoleRequest request);
        Task<bool> RemoveWorkspaceMemberAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId);
        Task<RoleSummaryDto> CreateWorkspaceRoleAsync(Guid userId, Guid workspaceId, CreateWorkspaceRoleRequest request);
        Task<List<RoleSummaryDto>> GetWorkspaceRolesSummaryAsync(Guid userId, Guid workspaceId);
        Task<RoleSummaryDto> GetWorkspaceRoleByIdSummaryAsync(Guid userId, Guid workspaceId, Guid roleId);
    }
}
