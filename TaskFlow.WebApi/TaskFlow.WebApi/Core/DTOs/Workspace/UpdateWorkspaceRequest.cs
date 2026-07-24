namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateWorkspaceRequest
    {
        public required string SpaceName { get; set; }
        public string? SpaceDescription { get; set; }
    }
}
