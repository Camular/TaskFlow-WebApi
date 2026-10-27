using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/workspaces/{workspaceId}/activity")]
    [Authorize]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogsController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
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
        // List activity in a workspace.
        // ====================================================================================================

        [HttpGet]

        public async Task<IActionResult> GetWorkspaceActivityAsync([FromRoute] Guid workspaceId)
        {
            var userId = GetCurrentUserId();

            var activity = await _activityLogService.GetWorkspaceActivityAsync(userId, workspaceId);

            return Ok(activity);
        }
    }
}
