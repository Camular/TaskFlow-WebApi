using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class AddWorkspaceMemberRequest
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        public Guid RoleId { get; set; }
    }
}