using System.ComponentModel.DataAnnotations;

namespace TaskFlow.WebApi.Core.DTOs.Workspace
{
    public class CreateWorkspaceRequest
    {
        [Required(ErrorMessage = "Çalışma alanı adı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Çalışma alanı adı en fazla 50 karakter olabilir.")]
        public string SpaceName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? SpaceDescription { get; set; }
    }
}