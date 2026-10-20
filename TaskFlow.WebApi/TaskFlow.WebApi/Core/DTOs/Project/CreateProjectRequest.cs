using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Project
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Project key is required.")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Project key must be 2 to 10 characters.")]
        public required string Key { get; set; }
    }
}
