using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.DTOs.Comment;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/Tasks/{workspaceId}/tasks/{taskId}/comments")]
    [Authorize]

    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
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
        // List comments on a task.
        // ====================================================================================================

        [HttpGet]

        public async Task<IActionResult> GetTaskCommentsAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId)
        {
            var userId = GetCurrentUserId();

            var comments = await _commentService.GetTaskCommentsAsync(userId, workspaceId, taskId);

            return Ok(comments);
        }

        // ====================================================================================================
        // Commands
        // Create, update, and delete a comment on a task.
        // ====================================================================================================

        [HttpPost]

        public async Task<IActionResult> CreateCommentAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromBody] CreateCommentRequest request)
        {
            var userId = GetCurrentUserId();

            var comment = await _commentService.CreateCommentAsync(userId, workspaceId, taskId, request);

            return Created($"/api/Tasks/{workspaceId}/tasks/{taskId}/comments/{comment.Id}", comment);
        }

        [HttpPut("{commentId}")]

        public async Task<IActionResult> UpdateCommentAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid commentId, [FromBody] UpdateCommentRequest request)
        {
            var userId = GetCurrentUserId();

            var comment = await _commentService.UpdateCommentAsync(userId, workspaceId, taskId, commentId, request);

            return Ok(comment);
        }

        [HttpDelete("{commentId}")]

        public async Task<IActionResult> DeleteCommentAsync([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid commentId)
        {
            var userId = GetCurrentUserId();

            await _commentService.DeleteCommentAsync(userId, workspaceId, taskId, commentId);

            return NoContent();
        }
    }
}
