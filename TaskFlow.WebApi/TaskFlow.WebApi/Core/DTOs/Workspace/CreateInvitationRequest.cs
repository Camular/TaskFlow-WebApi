using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class CreateInvitationRequest
    {

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        public required Guid RoleId { get; set; }
    }
}
