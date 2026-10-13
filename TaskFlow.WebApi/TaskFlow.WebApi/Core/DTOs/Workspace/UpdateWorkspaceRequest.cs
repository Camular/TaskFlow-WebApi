using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateWorkspaceRequest
    {
        [Required(ErrorMessage = "Workspace name is required.")]
        [StringLength(100, ErrorMessage = "Workspace name cannot be longer than 100 characters.")]
        public required string SpaceName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? SpaceDescription { get; set; }
    }
}
