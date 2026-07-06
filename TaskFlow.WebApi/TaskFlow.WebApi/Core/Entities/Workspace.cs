namespace TaskFlow.WebApi.Core.Entities
{
    public class Workspace
    {
        public Guid Id { get; set; }
        public required string SpaceName { get; set; }
        public string? SpaceDescription { get; set; }
        public bool IsPersonal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
