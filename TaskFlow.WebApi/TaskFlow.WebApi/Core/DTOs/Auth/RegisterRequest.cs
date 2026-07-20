using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Auth
{
    public class RegisterRequest
    {
        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters long")]
        public required string Username { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password confirmation is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password confirmation must be at least 6 characters long")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string PasswordConfirmed { get; set; }
        
    }
}
