using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using System.Text;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;
using TaskFlow.WebApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();

builder.Services.AddScoped<ITaskItemService, TaskItemService>();

builder.Services.AddScoped<IProjectService, ProjectService>();

builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

builder.Services.AddScoped<IBoardService, BoardService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var trustPlatformProxy = builder.Configuration.GetValue("ForwardedHeaders:TrustPlatformProxy", false);
var forwardLimit = builder.Configuration.GetValue("ForwardedHeaders:ForwardLimit", 1);
var knownNetworkCidrs = (builder.Configuration.GetSection("ForwardedHeaders:KnownIPNetworks").Get<string[]>()
    ?? Array.Empty<string>())
    .Where(cidr => !string.IsNullOrWhiteSpace(cidr))
    .Select(cidr => cidr.Trim())
    .ToArray();

if (forwardLimit < 1)
{
    throw new InvalidOperationException("ForwardedHeaders:ForwardLimit must be at least 1.");
}

// TrustPlatformProxy clears the proxy allow-list. Combining it with CIDRs would look
// like "trust Render" but would actually trust only those CIDRs.
if (trustPlatformProxy && knownNetworkCidrs.Length > 0)
{
    throw new InvalidOperationException(
        "ForwardedHeaders:TrustPlatformProxy cannot be combined with ForwardedHeaders:KnownIPNetworks.");
}

// With an empty allow-list the middleware does not check each hop. A limit above 1
// would accept client-supplied X-Forwarded-For values to the left of the proxy's entry.
if (trustPlatformProxy && forwardLimit != 1)
{
    throw new InvalidOperationException(
        "ForwardedHeaders:ForwardLimit must be 1 when ForwardedHeaders:TrustPlatformProxy is true.");
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = forwardLimit;

    if (trustPlatformProxy)
    {
        // Render does not publish the load-balancer address. Empty lists trust the
        // socket peer. Safe only when the container port is not reachable from the internet.
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    }

    foreach (var cidr in knownNetworkCidrs)
    {
        if (!System.Net.IPNetwork.TryParse(cidr, out var network))
        {
            throw new InvalidOperationException(
                $"ForwardedHeaders:KnownIPNetworks contains an invalid CIDR '{cidr}'.");
        }

        options.KnownIPNetworks.Add(network);
    }
});

var rateLimitPermit = builder.Configuration.GetValue("RateLimiting:PermitLimit", 100);
var rateLimitWindowSeconds = builder.Configuration.GetValue("RateLimiting:WindowSeconds", 60);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = StatusCodes.Status429TooManyRequests,
            message = "Too many requests. Please try again later."
        }, cancellationToken);
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        if (httpContext.Request.Path.StartsWithSegments("/health"))
        {
            return RateLimitPartition.GetNoLimiter("health");
        }

        // UseForwardedHeaders rewrites this before the limiter runs. Otherwise it is the proxy.
        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimitPermit,
            Window = TimeSpan.FromSeconds(rateLimitWindowSeconds),
            QueueLimit = 0
        });
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (trustPlatformProxy)
{
    app.Logger.LogWarning(
        "Forwarded headers trust the connecting proxy (ForwardedHeaders:TrustPlatformProxy). ForwardLimit is {ForwardLimit}.",
        forwardLimit);
}

app.UseMiddleware<TaskFlow.WebApi.Infrastructure.Middleware.ExceptionMiddleware>();

// Must run before UseHttpsRedirection and UseRateLimiter so scheme and client IP are already rewritten.
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
