using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Workspace;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Tests.Support;

namespace TaskFlow.WebApi.Tests;

public class WorkspaceServiceTests
{
    [Fact]
    public async Task UpdateWorkspace_ViewerIsForbidden_OwnerSucceeds()
    {
        using var scope = new SqliteServiceScope();
        var owner = await scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Ada", "ada@example.com"));
        var viewer = await scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Bea", "bea@example.com"));
        var workspaceId = await OwnerWorkspaceId(scope, owner.UserId);
        await AddMember(scope, viewer.UserId, workspaceId, SystemRoles.ViewerId);

        var forbidden = await Assert.ThrowsAsync<ForbiddenException>(() =>
            scope.Workspaces.UpdateWorkspaceAsync(viewer.UserId, workspaceId, new UpdateWorkspaceRequest
            {
                SpaceName = "Viewer rename"
            }));

        Assert.Contains("workspace.update", forbidden.Message);
        Assert.Equal("Ada's Workspace", await WorkspaceName(scope, workspaceId));

        var updated = await scope.Workspaces.UpdateWorkspaceAsync(owner.UserId, workspaceId, new UpdateWorkspaceRequest
        {
            SpaceName = "Renamed",
            SpaceDescription = "Updated by the owner"
        });

        Assert.Equal("Renamed", updated.SpaceName);
        Assert.Equal(SystemRoles.OwnerName, updated.RoleName);
        Assert.Equal("Renamed", await WorkspaceName(scope, workspaceId));
    }

    [Fact]
    public async Task RemoveWorkspaceMember_LastOwner_Throws()
    {
        using var scope = new SqliteServiceScope();
        var owner = await scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Ada", "ada@example.com"));
        var workspaceId = await OwnerWorkspaceId(scope, owner.UserId);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            scope.Workspaces.RemoveWorkspaceMemberAsync(owner.UserId, workspaceId, owner.UserId));

        Assert.Equal(
            "The only Owner of the workspace cannot be removed. Transfer ownership to another member first.",
            exception.Message);

        var stillOwner = await scope.Context.UserWorkspaceRoles.AnyAsync(uwr =>
            uwr.UserId == owner.UserId &&
            uwr.WorkspaceId == workspaceId &&
            uwr.RoleId == SystemRoles.OwnerId);
        Assert.True(stillOwner);
    }

    private static async Task<Guid> OwnerWorkspaceId(SqliteServiceScope scope, Guid userId)
    {
        return await scope.Context.UserWorkspaceRoles
            .Where(uwr => uwr.UserId == userId && uwr.RoleId == SystemRoles.OwnerId)
            .Select(uwr => uwr.WorkspaceId)
            .SingleAsync();
    }

    private static async Task AddMember(SqliteServiceScope scope, Guid userId, Guid workspaceId, Guid roleId)
    {
        var user = await scope.Context.Users.SingleAsync(u => u.Id == userId);
        var workspace = await scope.Context.Workspaces.SingleAsync(w => w.Id == workspaceId);
        var role = await scope.Context.WorkspaceRoles.SingleAsync(r => r.Id == roleId);

        scope.Context.UserWorkspaceRoles.Add(new UserWorkspaceRole
        {
            UserId = user.Id,
            WorkspaceId = workspace.Id,
            RoleId = role.Id,
            User = user,
            Workspace = workspace,
            Role = role
        });

        await scope.Context.SaveChangesAsync();
    }

    private static async Task<string> WorkspaceName(SqliteServiceScope scope, Guid workspaceId)
    {
        return await scope.Context.Workspaces
            .Where(w => w.Id == workspaceId)
            .Select(w => w.SpaceName)
            .SingleAsync();
    }
}
