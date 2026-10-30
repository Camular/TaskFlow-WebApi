using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Board
{
    public class UpdateColumnRequest
    {
        [Required(ErrorMessage = "Column name is required.")]
        [StringLength(100, ErrorMessage = "Column name cannot be longer than 100 characters.")]
        public required string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Column position cannot be negative.")]
        public int Position { get; set; }
    }
}
