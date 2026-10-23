using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Comment
{
    public class UpdateCommentRequest
    {
        [Required(ErrorMessage = "Comment body is required.")]
        [StringLength(2000, ErrorMessage = "Comment body cannot be longer than 2000 characters.")]
        public required string Body { get; set; }
    }
}
