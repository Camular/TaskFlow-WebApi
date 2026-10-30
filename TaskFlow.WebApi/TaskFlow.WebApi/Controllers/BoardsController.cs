using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.DTOs.Board;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/workspaces/{workspaceId}/boards")]
    [Authorize]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardsController(IBoardService boardService)
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
        // Board queries
        // List boards in a workspace, and get one board by id.
        // ====================================================================================================

        [HttpGet]
        public async Task<IActionResult> GetWorkspaceBoardsAsync([FromRoute] Guid workspaceId)
        {
            var userId = GetCurrentUserId();

            var boards = await _boardService.GetWorkspaceBoardsAsync(userId, workspaceId);

            return Ok(boards);
        }

        [HttpGet("{boardId}")]
        [ActionName(nameof(GetBoardByIdAsync))]
        public async Task<IActionResult> GetBoardByIdAsync([FromRoute] Guid workspaceId, [FromRoute] Guid boardId)
        {
            var userId = GetCurrentUserId();

            var board = await _boardService.GetBoardByIdAsync(userId, workspaceId, boardId);

            return Ok(board);
        }

        // ====================================================================================================
        // Board commands
        // Create, update, and delete a board in a workspace.
        // ====================================================================================================

        [HttpPost]
        public async Task<IActionResult> CreateBoardAsync([FromRoute] Guid workspaceId, [FromBody] CreateBoardRequest request)
        {
            var userId = GetCurrentUserId();

            var board = await _boardService.CreateBoardAsync(userId, workspaceId, request);

            return Created($"/api/workspaces/{workspaceId}/boards/{board.Id}", board);
        }

        [HttpPut("{boardId}")]
        public async Task<IActionResult> UpdateBoardAsync([FromRoute] Guid workspaceId, [FromRoute] Guid boardId, [FromBody] UpdateBoardRequest request)
        {
            var userId = GetCurrentUserId();

            var board = await _boardService.UpdateBoardAsync(userId, workspaceId, boardId, request);

            return Ok(board);
        }

        [HttpDelete("{boardId}")]
        public async Task<IActionResult> DeleteBoardAsync([FromRoute] Guid workspaceId, [FromRoute] Guid boardId)
        {
            var userId = GetCurrentUserId();

            await _boardService.DeleteBoardAsync(userId, workspaceId, boardId);

            return NoContent();
        }
    }
}
