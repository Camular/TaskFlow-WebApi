namespace TaskFlow.WebApi.Core.DTOs.Project
{
    public class ProjectSummaryDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Key { get; set; }
    }
}
