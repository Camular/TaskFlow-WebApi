using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class AcceptInvitationRequest
    {
        [Required(ErrorMessage = "Invitation token is required.")]
        [StringLength(100, ErrorMessage = "Token cannot be longer than 100 characters.")]
        public required string Token { get; set; }
    }
}
