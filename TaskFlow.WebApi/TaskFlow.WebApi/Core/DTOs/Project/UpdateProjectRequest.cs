using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Project
{
    public class UpdateProjectRequest
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(100, ErrorMessage = "Project name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }
    }
}
