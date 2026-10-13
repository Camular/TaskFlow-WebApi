using TaskFlow.WebApi.Core.DTOs.Project;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IProjectService
    {
        // Projects — create a project, list projects, get one project by id, update a project, and delete a project.
        Task<List<ProjectSummaryDto>> GetWorkspaceProjectsAsync(Guid userId, Guid workspaceId);
        Task<ProjectSummaryDto> GetProjectByIdAsync(Guid userId, Guid workspaceId, Guid projectId);
        Task<ProjectSummaryDto> CreateProjectAsync(Guid userId, Guid workspaceId, CreateProjectRequest request);
        Task<ProjectSummaryDto> UpdateProjectAsync(Guid userId, Guid workspaceId, Guid projectId, UpdateProjectRequest request);
        Task<bool> DeleteProjectAsync(Guid userId, Guid workspaceId, Guid projectId);
    }
}
