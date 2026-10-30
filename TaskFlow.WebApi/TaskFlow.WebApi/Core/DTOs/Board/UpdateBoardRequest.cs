using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Board
{
    public class UpdateBoardRequest
    {
        [Required(ErrorMessage = "Board name is required.")]
        [StringLength(100, ErrorMessage = "Board name cannot be longer than 100 characters.")]
        public required string Name { get; set; }
    }
}
