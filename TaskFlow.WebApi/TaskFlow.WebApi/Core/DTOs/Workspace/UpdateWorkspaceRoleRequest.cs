using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateWorkspaceRoleRequest
    {
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, ErrorMessage = "Role name cannot be longer than 50 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Select at least one permission.")]
        [MinLength(1, ErrorMessage = "Select at least one permission.")]
        public required List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
}
