namespace TaskFlow.WebApi.Core.DTOs.Comment
{
    public class CommentSummaryDto
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public Guid AuthorUserId { get; set; }
        public required string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
