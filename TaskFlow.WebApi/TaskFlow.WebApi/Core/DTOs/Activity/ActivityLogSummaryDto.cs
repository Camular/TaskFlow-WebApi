namespace TaskFlow.WebApi.Core.DTOs.Activity
{
    public class ActivityLogSummaryDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid ActorUserId { get; set; }
        public Guid? TaskItemId { get; set; }
        public required string Action { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
