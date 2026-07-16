using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Workspace> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.SpaceName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.SpaceDescription)
                .HasMaxLength(500);

            builder.Property(w => w.IsPersonal)
                .HasDefaultValue(false);

            builder.Property(w => w.CreatedAt)
                .HasColumnType("timestamp with time zone"); 
        }
    }
}
