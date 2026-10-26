using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.CreatedAt)
               .HasColumnType("timestamp with time zone");

            builder.HasOne(a => a.TaskItem)
                .WithMany()
                .HasForeignKey(a => a.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
