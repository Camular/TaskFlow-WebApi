using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class UpdateWorkspaceRequest
    {
        [Required(ErrorMessage = "Çalışma alanı adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Çalışma alanı adı en fazla 100 karakter olabilir.")]
        public required string SpaceName { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? SpaceDescription { get; set; }
    }
}
