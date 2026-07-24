using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class WorkspaceSummaryDto
    {
        public Guid Id { get; set; }
        public required string SpaceName { get; set; }
        public required string SpaceDescription { get; set; }
        public required string RoleName { get; set; }
        public bool IsPersonal { get; set; }

    }
}
