namespace TaskFlow.WebApi.Core.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public Guid UploadedByUserId { get; set; }
        public required string OriginalFileName { get; set; }
        public required string StoredFileName { get; set; }
        public required string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required TaskItem TaskItem { get; set; }
        public required User UploadedBy { get; set; }

    }
}
