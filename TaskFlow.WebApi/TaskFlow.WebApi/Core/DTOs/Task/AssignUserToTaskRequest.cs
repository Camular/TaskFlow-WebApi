using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Task
{
    public class AssignUserToTaskRequest
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public required string Email { get; set; }

    }
}
