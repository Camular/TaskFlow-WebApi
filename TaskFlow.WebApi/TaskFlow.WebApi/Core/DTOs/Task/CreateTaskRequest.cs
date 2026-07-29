using System.ComponentModel.DataAnnotations;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Core.DTOs.Task
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Bir başlık belirleyin.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public required string Title { get; set; }

        [StringLength(500, ErrorMessage = "Tanım en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
