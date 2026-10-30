using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.DTOs.Board;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/workspaces/{workspaceId}/boards/{boardId}/columns")]
    [Authorize]
    public class ColumnsController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public ColumnsController(IBoardService boardService)
        {
            _boardService = boardService;
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
        // Column queries
        // List columns on a board.
        // ====================================================================================================

        [HttpGet]
        public async Task<IActionResult> GetBoardColumnsAsync([FromRoute] Guid workspaceId, [FromRoute] Guid boardId)
        {
            var userId = GetCurrentUserId();

            var columns = await _boardService.GetBoardColumnsAsync(userId, workspaceId, boardId);

            return Ok(columns);
        }

        // ====================================================================================================
        // Column commands
        // Create, update, and delete a column on a board.
        // ====================================================================================================

        [HttpPost]
        public async Task<IActionResult> CreateColumnAsync([FromRoute] Guid workspaceId, [FromRoute] Guid boardId, [FromBody] CreateColumnRequest request)
        {
            var userId = GetCurrentUserId();

            var column = await _boardService.CreateColumnAsync(userId, workspaceId, boardId, request);

            return Created($"/api/workspaces/{workspaceId}/boards/{boardId}/columns/{column.Id}", column);
        }

        [HttpPut("{columnId}")]
        public async Task<IActionResult> UpdateColumnAsync(
            [FromRoute] Guid workspaceId,
            [FromRoute] Guid boardId,
            [FromRoute] Guid columnId,
            [FromBody] UpdateColumnRequest request)
        {
            var userId = GetCurrentUserId();

            var column = await _boardService.UpdateColumnAsync(userId, workspaceId, boardId, columnId, request);

            return Ok(column);
        }

        [HttpDelete("{columnId}")]
        public async Task<IActionResult> DeleteColumnAsync(
            [FromRoute] Guid workspaceId,
            [FromRoute] Guid boardId,
            [FromRoute] Guid columnId)
        {
            var userId = GetCurrentUserId();

            await _boardService.DeleteColumnAsync(userId, workspaceId, boardId, columnId);

            return NoContent();
        }
    }
}
