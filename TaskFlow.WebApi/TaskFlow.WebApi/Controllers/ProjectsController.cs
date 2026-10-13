using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.WebApi.Core.DTOs.Project;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;

namespace TaskFlow.WebApi.Controllers
{
    [ApiController]
    [Route("api/workspaces/{workspaceId}/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
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
        // List projects in a workspace, and get one project by id.
        // ====================================================================================================

        [HttpGet]

        public async Task<IActionResult> GetWorkspaceProjectsAsync([FromRoute] Guid workspaceId)
        {
            var userId = GetCurrentUserId();

            var projects = await _projectService.GetWorkspaceProjectsAsync(userId, workspaceId);

            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        [ActionName(nameof(GetProjectByIdAsync))]

        public async Task<IActionResult> GetProjectByIdAsync([FromRoute] Guid workspaceId, [FromRoute] Guid projectId)
        {
            var userId = GetCurrentUserId();

            var project = await _projectService.GetProjectByIdAsync(userId, workspaceId, projectId);

            return Ok(project);
        }


        // ====================================================================================================
        // Commands
        // Create, update, and delete a project in a workspace.
        // ====================================================================================================

        [HttpPost]

        public async Task<IActionResult> CreateProjectAsync([FromRoute] Guid workspaceId, [FromBody] CreateProjectRequest request)
        {
            var userId = GetCurrentUserId();

            var project = await _projectService.CreateProjectAsync(userId, workspaceId, request);

            return Created($"/api/workspaces/{workspaceId}/projects/{project.Id}", project);
        }

        [HttpPut("{projectId}")]


        public async Task<IActionResult> UpdateProjectAsync([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request)
        {
            var userId = GetCurrentUserId();

            var project = await _projectService.UpdateProjectAsync(userId, workspaceId, projectId, request);

            return Ok(project);
        }

        [HttpDelete("{projectId}")]


        public async Task<IActionResult> DeleteProjectAsync([FromRoute] Guid workspaceId, [FromRoute] Guid projectId)
        {
            var userId = GetCurrentUserId();

            var project = await _projectService.DeleteProjectAsync(userId, workspaceId, projectId);

            return NoContent();
        }
    }
}
