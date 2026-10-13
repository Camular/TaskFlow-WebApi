using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class CreateWorkspaceRequest
    {
        [Required(ErrorMessage = "Workspace name is required.")]
        [StringLength(50, ErrorMessage = "Workspace name cannot be longer than 50 characters.")]
        public string SpaceName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? SpaceDescription { get; set; }
    }
}