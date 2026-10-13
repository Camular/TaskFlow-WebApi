using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateMemberRoleRequest
    {
        [Required(ErrorMessage = "A role is required.")]
        public Guid RoleId { get; set; }
    }
}