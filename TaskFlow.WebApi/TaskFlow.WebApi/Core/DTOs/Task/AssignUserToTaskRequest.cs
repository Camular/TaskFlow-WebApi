using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Task
{
    public class AssignUserToTaskRequest
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public required string Email { get; set; }

    }
}
