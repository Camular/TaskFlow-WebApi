using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class CreateInvitationRequest
    {

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A role is required.")]
        public required Guid RoleId { get; set; }
    }
}
