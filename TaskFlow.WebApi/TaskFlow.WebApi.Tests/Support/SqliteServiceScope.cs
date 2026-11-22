using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskFlow.WebApi.Core.DTOs.Auth;
using TaskFlow.WebApi.Infrastructure.Data;
using TaskFlow.WebApi.Infrastructure.Services;

namespace TaskFlow.WebApi.Tests.Support;

// Each test gets its own in-memory SQLite database. The connection stays open
// for the test; closing it drops the database. The local PostgreSQL database is not used.
internal sealed class SqliteServiceScope : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Context { get; }
    public AuthService Auth { get; }
    public WorkspaceService Workspaces { get; }

    public SqliteServiceScope()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "test-signing-key-at-least-32-chars",
                ["Jwt:Issuer"] = "TaskFlowTests",
                ["Jwt:Audience"] = "TaskFlowTests",
                ["Jwt:ExpireMinutes"] = "30"
            })
            .Build();

        Auth = new AuthService(configuration, Context);
        Workspaces = new WorkspaceService(Context);
    }

    public static RegisterRequest RegisterRequest(string username, string email, string password = "secret1")
    {
        return new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password,
            PasswordConfirmed = password
        };
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
