using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Auth
{
    public class LoginRequest
    {
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")] //burdaki required ASP.NET Core Framework kuralıdır.
        public required string Email { get; set; } //burdaki required C# Derleyicisinin (Compiler) kuralıdır.

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
