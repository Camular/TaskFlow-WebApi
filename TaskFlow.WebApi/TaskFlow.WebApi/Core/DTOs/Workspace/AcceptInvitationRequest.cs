using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class AcceptInvitationRequest
    {
        [Required(ErrorMessage = "Invitation Tokenı bulunması zorunludur.")]
        [StringLength(100, ErrorMessage = "Token en fazla 100 karakter olabilir.")]
        public required string Token { get; set; }
    }
}
