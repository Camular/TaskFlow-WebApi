using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/Tasks/{workspaceId}/tasks/{taskId}/attachments")]
    [Authorize]

    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentsController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        // ====================================================================================================
        // Current user
        // Reads the signed-in user id from the access token.
        // ====================================================================================================

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("The access token is missing, invalid, or expired.");
            }
            return Guid.Parse(userIdClaim);
        }


        // ====================================================================================================
        // Queries
        // List attachments on a task.
        // ====================================================================================================

        [HttpGet]

        public async Task<IActionResult> GetTaskAttachmentsAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId)
        {
            var userId = GetCurrentUserId();

            var attachments = await _attachmentService.GetTaskAttachmentsAsync(userId, workspaceId, taskId);

            return Ok(attachments);
        }

        // ====================================================================================================
        // Commands
        // Upload a file to a task, and delete an attachment.
        // ====================================================================================================

        [HttpPost]

        public async Task<IActionResult> UploadAttachmentAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, IFormFile file)
        {
            var userId = GetCurrentUserId();

            var attachment = await _attachmentService.UploadAttachmentAsync(userId, workspaceId, taskId, file);

            return Created($"/api/Tasks/{workspaceId}/tasks/{taskId}/attachments/{attachment.Id}", attachment);
        }

        [HttpDelete("{attachmentId}")]

        public async Task<IActionResult> DeleteAttachmentAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid attachmentId)
        {
            var userId = GetCurrentUserId();

            await _attachmentService.DeleteAttachmentAsync(userId, workspaceId, taskId, attachmentId);

            return NoContent();
        }
    }
}
