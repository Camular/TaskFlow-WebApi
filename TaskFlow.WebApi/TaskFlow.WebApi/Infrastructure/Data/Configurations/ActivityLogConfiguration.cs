using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.CreatedAt)
               .HasColumnType("timestamp with time zone");

            builder.HasOne(a => a.Workspace)
                .WithMany()
                .HasForeignKey(a => a.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(a => a.Actor)
                .WithMany()
                .HasForeignKey(a => a.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.TaskItem)
                .WithMany()
                .HasForeignKey(a => a.TaskItemId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
