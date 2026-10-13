using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Task;

public class UpdateTaskRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public required string Title { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }
}