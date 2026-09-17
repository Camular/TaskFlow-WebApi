using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateWorkspaceRoleRequest
    {
        [Required(ErrorMessage = "Rol adı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "En az bir izin seçilmelidir.")]
        [MinLength(1, ErrorMessage = "En az bir izin seçilmelidir.")]
        public required List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
}
