namespace TaskFlow.WebApi.Core.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public Guid AuthorUserId { get; set; }
        public required string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public required TaskItem TaskItem { get; set; }
        public required User Author { get; set; }

    }
}
