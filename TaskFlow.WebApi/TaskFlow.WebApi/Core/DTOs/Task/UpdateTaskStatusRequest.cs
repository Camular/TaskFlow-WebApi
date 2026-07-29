using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskFlow.WebApi.Core.Entities.TaskStatus; 

namespace TaskFlow.WebApi.Core.DTOs.Task;

public class UpdateTaskStatusRequest
{
    [Required(ErrorMessage = "Görev durumu zorunludur.")]
    [EnumDataType(typeof(TaskStatus), ErrorMessage = "Geçersiz bir görev durumu girdiniz.")]
    public required TaskStatus Status { get; set; }
}