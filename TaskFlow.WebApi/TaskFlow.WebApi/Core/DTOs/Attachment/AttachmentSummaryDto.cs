namespace TaskFlow.WebApi.Core.DTOs.Attachment
{
    public class AttachmentSummaryDto
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public Guid UploadedByUserId { get; set; }
        public required string OriginalFileName { get; set; }
        public required string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
