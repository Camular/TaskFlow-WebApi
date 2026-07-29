using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.DTOs.Task;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;


namespace TaskFlow.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;

        public TasksController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("Geçersiz veya süresi dolmuş token.");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpGet]

        public async Task<IActionResult> GetMyTasksAsync()
        {
            var userId = GetCurrentUserId();

            var usertasks = await _taskItemService.GetUserTaskSummaryAsync(userId);

            return Ok(usertasks);
        }

        [HttpGet("{workspaceId}/tasks")]

        public async Task<IActionResult> GetWorkspaceTasksAsync([FromRoute] Guid workspaceId)
        {
            var userId = GetCurrentUserId();

            var workspacetasks = await _taskItemService.GetWorkspaceTaskSummaryAsync(userId, workspaceId);

            return Ok(workspacetasks);
        }

        [HttpGet("{workspaceId}/tasks/{taskId}")]
        public async Task<IActionResult> GetTaskByIdAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId)
        {
            var userId = GetCurrentUserId();

            var task = await _taskItemService.GetTaskByIdAsync(userId, workspaceId, taskId);

            return Ok(task);
        }

        [HttpPost("{workspaceId}/tasks")]

        public async Task<IActionResult> CreateTaskAsync([FromRoute] Guid workspaceId, [FromBody] CreateTaskRequest request)
        {
            var userId = GetCurrentUserId();

            var createTask = await _taskItemService.CreateTaskAsync(userId, workspaceId, request);

            return CreatedAtAction(nameof(GetTaskByIdAsync), new { workspaceId, taskId = createTask.Id }, createTask);
        }

        [HttpPut("{workspaceId}/tasks/{taskId}")]

        public async Task<IActionResult> UpdateTaskAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromBody] UpdateTaskRequest request)
        {
            var userId = GetCurrentUserId();

            var updatedTask = await _taskItemService.UpdateTaskAsync(userId, workspaceId, taskId, request);

            return Ok(updatedTask);
        }

        [HttpDelete("{workspaceId}/tasks/{taskId}")]

        public async Task<IActionResult> DeleteTaskAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId)
        {
            var userId = GetCurrentUserId();

            var deletedTask = await _taskItemService.DeleteTaskAsync(userId, workspaceId, taskId);

            return NoContent();
        }

        [HttpPatch("{workspaceId}/tasks/{taskId}/assign")]

        public async Task<IActionResult> AssignUserToTaskAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromBody] AssignUserToTaskRequest request)
        {
            var userId = GetCurrentUserId();

            var assignedTask = await _taskItemService.AssignUserToTaskAsync(userId, workspaceId, taskId, request);

            return Ok(assignedTask);
        }

        [HttpPatch("{workspaceId}/tasks/{taskId}/deadline")]

        public async Task<IActionResult> UpdateTaskDeadlineAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromBody] UpdateTaskDeadlineRequest request)
        {
            var userId = GetCurrentUserId();

            var updatedTask = await _taskItemService.UpdateTaskDeadlineAsync(userId, workspaceId, taskId, request);

            return Ok(updatedTask);
        }

        [HttpPatch("{workspaceId}/tasks/{taskId}/status")]

        public async Task<IActionResult> UpdateTaskStatusAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            var userId = GetCurrentUserId();

            var updatedTask = await _taskItemService.UpdateTaskStatusAsync(userId, workspaceId, taskId, request);

            return Ok(updatedTask);
        }
    }
}
