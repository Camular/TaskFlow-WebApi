using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskFlow.WebApi.Core.Entities.TaskStatus; 

namespace TaskFlow.WebApi.Core.DTOs.Task;

public class UpdateTaskStatusRequest
{
    [Required(ErrorMessage = "Task status is required.")]
    [EnumDataType(typeof(TaskStatus), ErrorMessage = "The task status is not valid.")]
    public required TaskStatus Status { get; set; }
}