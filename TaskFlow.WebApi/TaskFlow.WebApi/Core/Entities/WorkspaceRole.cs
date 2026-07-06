namespace TaskFlow.WebApi.Core.Entities
{
    public class WorkspaceRole
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Name { get; set; }

    }
}
