using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceRole> WorkspaceRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<UserWorkspaceRole> UserWorkspaceRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserWorkspaceRole>()
                .HasIndex(uwr => new { uwr.UserId, uwr.RoleId, uwr.WorkspaceId })
                .IsUnique();

            modelBuilder.Entity<WorkspaceRole>()
                .HasIndex(wr => new { wr.Name })
                .IsUnique()
                .HasFilter("\"WorkspaceId\" IS NULL");

            modelBuilder.Entity<WorkspaceRole>()
                .HasIndex(wr => new { wr.Name, wr.WorkspaceId })
                .IsUnique()
                .HasFilter("\"WorkspaceId\" IS NOT NULL");

            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();
        }

    }
}
