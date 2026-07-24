using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateMemberRoleRequest
    {
        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        public Guid RoleId { get; set; }
    }
}