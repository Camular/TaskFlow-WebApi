using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Email).HasMaxLength(100).IsRequired();

            builder.Property(i => i.Token).HasMaxLength(100).IsRequired();

            builder.HasIndex(i => i.Token).IsUnique();

            builder.Property(i => i.CreatedAt)
                .HasColumnType("timestamp with time zone");

            builder.Property(i => i.ExpiresAt)
                .HasColumnType("timestamp with time zone");

            builder.HasOne(i => i.Workspace)
                .WithMany()
                .HasForeignKey(i => i.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.InvitedBy)
                .WithMany()
                .HasForeignKey(i => i.InvitedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Role)
                .WithMany()
                .HasForeignKey(i => i.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
