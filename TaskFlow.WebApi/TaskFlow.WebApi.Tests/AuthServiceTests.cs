using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Tests.Support;

namespace TaskFlow.WebApi.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task Register_CreatesUserPersonalWorkspaceAndOwnerRole()
    {
        using var scope = new SqliteServiceScope();

        var response = await scope.Auth.RegisterAsync(
            SqliteServiceScope.RegisterRequest("Ada", "Ada@Example.com"));

        Assert.Equal("Ada", response.UserName);
        Assert.Equal("ada@example.com", response.Email);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));

        var user = await scope.Context.Users.SingleAsync();
        Assert.Equal("Ada", user.Username);
        Assert.Equal("ada@example.com", user.Email);
        Assert.NotEqual("secret1", user.PasswordHash);
        Assert.StartsWith("$2", user.PasswordHash);

        var membership = await scope.Context.UserWorkspaceRoles
            .Include(uwr => uwr.Workspace)
            .Include(uwr => uwr.Role)
            .SingleAsync();

        Assert.Equal(user.Id, membership.UserId);
        Assert.Equal(SystemRoles.OwnerId, membership.RoleId);
        Assert.Equal(SystemRoles.OwnerName, membership.Role.Name);
        Assert.True(membership.Workspace.IsPersonal);
        Assert.Equal("Ada's Workspace", membership.Workspace.SpaceName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsConflictException()
    {
        using var scope = new SqliteServiceScope();
        await scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Ada", "ada@example.com"));

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Other", "ADA@example.com")));

        Assert.Equal("Email already exists.", exception.Message);
        Assert.Equal(1, await scope.Context.Users.CountAsync());
    }

    [Fact]
    public async Task Register_DuplicateUsername_ThrowsConflictException()
    {
        using var scope = new SqliteServiceScope();
        await scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Ada", "ada@example.com"));

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("ada", "other@example.com")));

        Assert.Equal("Username already exists.", exception.Message);
        Assert.Equal(1, await scope.Context.Users.CountAsync());
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorizedException()
    {
        using var scope = new SqliteServiceScope();
        await scope.Auth.RegisterAsync(SqliteServiceScope.RegisterRequest("Ada", "ada@example.com"));

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            scope.Auth.LoginAsync(new TaskFlow.WebApi.Core.DTOs.Auth.LoginRequest
            {
                Email = "ada@example.com",
                Password = "wrong-password"
            }));

        Assert.Equal("Invalid email or password.", exception.Message);
    }
}
