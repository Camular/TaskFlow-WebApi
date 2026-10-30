namespace TaskFlow.WebApi.Core.DTOs.Board
{
    public class BoardSummaryDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
