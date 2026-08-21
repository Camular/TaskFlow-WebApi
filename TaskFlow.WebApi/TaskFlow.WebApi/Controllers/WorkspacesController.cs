using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.WebApi.Core.DTOs.Workspace;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Core.Exceptions;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspacesController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
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
        public async Task<IActionResult> GetMyWorkspacesAsync()
        {
            var userId = GetCurrentUserId();

            var workspaces = await _workspaceService.GetUserWorkspaceSummaryAsync(userId);

            return Ok(workspaces);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkspaceAsync([FromBody] CreateWorkspaceRequest request)
        {
            var userId = GetCurrentUserId();

            var workspace = await _workspaceService.CreateWorkspaceAsync(userId, request);

            return CreatedAtAction(nameof(GetWorkspaceByIdAsync), new { id = workspace.Id }, workspace);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkspaceAsync([FromBody] UpdateWorkspaceRequest request, [FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();

            var workspace = await _workspaceService.UpdateWorkspaceAsync(userId, id, request);

            return Ok(workspace);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkspaceAsync([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();

            await _workspaceService.DeleteWorkspaceAsync(userId, id);

            return NoContent();
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetWorkspaceByIdAsync))]
        public async Task<IActionResult> GetWorkspaceByIdAsync([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();

            var workspace = await _workspaceService.GetWorkspaceByIdAsync(userId, id);

            return Ok(workspace);
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetWorkspaceMembersAsync([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();

            var workspace = await _workspaceService.GetWorkspaceMembersAsync(userId, id);

            return Ok(workspace);
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetWorkspaceRolesSummaryAsync([FromRoute] Guid id)
        {
            var userId = GetCurrentUserId();

            var roles = await _workspaceService.GetWorkspaceRolesSummaryAsync(userId, id);

            return Ok(roles);
        }

        [HttpGet("{id}/roles/{roleId}")]
        [ActionName(nameof(GetWorkspaceRoleByIdSummaryAsync))]
        public async Task<IActionResult> GetWorkspaceRoleByIdSummaryAsync([FromRoute] Guid id, [FromRoute] Guid roleId)
        {
            var userId = GetCurrentUserId();

            var role = await _workspaceService.GetWorkspaceRoleByIdSummaryAsync(userId, id, roleId);

            return Ok(role);
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddWorkspaceMemberAsync([FromRoute] Guid id, [FromBody] AddWorkspaceMemberRequest request)
        {
            var userId = GetCurrentUserId();

            var addMember = await _workspaceService.AddWorkspaceMemberAsync(userId, id, request);

            return Ok(addMember);
        }

        [HttpPut("{id}/members/{memberId}")]
        public async Task<IActionResult> UpdateMemberRoleAsync(
            [FromRoute] Guid id,
            [FromRoute] Guid memberId,
            [FromBody] UpdateMemberRoleRequest request)
        {
            var userId = GetCurrentUserId();

            var updatedMember = await _workspaceService.UpdateMemberRoleAsync(userId, id, memberId, request);

            return Ok(updatedMember);
        }

        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveWorkspaceMember([FromRoute] Guid id, [FromRoute] Guid memberId)
        {
            var userId = GetCurrentUserId();

            await _workspaceService.RemoveWorkspaceMemberAsync(userId, id, memberId);

            return NoContent();
        }

        [HttpPost("{id}/roles")]
        public async Task<IActionResult> CreateWorkspaceRoleAsync([FromRoute] Guid id, [FromBody] CreateWorkspaceRoleRequest request)
        {
            var userId = GetCurrentUserId();

            var role = await _workspaceService.CreateWorkspaceRoleAsync(userId, id, request);

            return CreatedAtAction(nameof(GetWorkspaceRoleByIdSummaryAsync), new { id , roleId = role.RoleId }, role);

        }
    }
}