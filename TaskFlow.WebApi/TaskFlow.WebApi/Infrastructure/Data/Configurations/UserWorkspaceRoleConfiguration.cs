using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Entities;


namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class UserWorkspaceRoleConfiguration : IEntityTypeConfiguration<UserWorkspaceRole>
    {
        public void Configure(EntityTypeBuilder<UserWorkspaceRole> builder)
        {
            builder.HasKey(uwr => new { uwr.UserId, uwr.RoleId, uwr.WorkspaceId });

            builder.HasOne(uwr => uwr.Workspace)
                .WithMany(w => w.UserWorkspaceRoles)
                .HasForeignKey(uwr => uwr.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uwr => uwr.User)
                .WithMany(u => u.UserWorkspaceRoles)
                .HasForeignKey(uwr => uwr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uwr => uwr.Role)
                .WithMany(r => r.UserWorkspaceRoles)
                .HasForeignKey(uwr => uwr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);



        }

        
    }
}
