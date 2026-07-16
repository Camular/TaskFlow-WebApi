using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class WorkspaceRoleConfiguration : IEntityTypeConfiguration<WorkspaceRole>
    {
        public void Configure(EntityTypeBuilder<WorkspaceRole> builder)
        {
            builder.HasKey(wr => wr.Id);

            builder.Property(wr => wr.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(wr => new { wr.Name })
                .IsUnique()
                .HasFilter("\"WorkspaceId\" IS NULL");

            builder.HasIndex(wr => new { wr.WorkspaceId, wr.Name })
                .IsUnique()
                .HasFilter("\"WorkspaceId\" IS NOT NULL");

            builder.HasData(
                new WorkspaceRole
                {
                    Id = SystemRoles.OwnerId,
                    Name = SystemRoles.OwnerName,
                },
                new WorkspaceRole
                {
                    Id = SystemRoles.AdministratorId,
                    Name = SystemRoles.AdministratorName,
                },
                new WorkspaceRole
                {
                    Id = SystemRoles.MemberId,
                    Name = SystemRoles.MemberName,
                },
                new WorkspaceRole
                {
                    Id = SystemRoles.ViewerId,
                    Name = SystemRoles.ViewerName,
                }
            );
                
        }
    }
}
