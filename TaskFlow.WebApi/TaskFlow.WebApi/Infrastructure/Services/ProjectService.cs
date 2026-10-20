using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Project;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;
using static TaskFlow.WebApi.Core.Authorization.SystemPermissions;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }


        // ====================================================================================================
        // Guards
        // Shared workspace access check used by the actions below.
        // ====================================================================================================

        private async Task<UserWorkspaceRole> CheckAndGetWorkspaceRoleAsync(Guid userId, Guid workspaceId, string permissionCode)
        {
            var userWorkspaceRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.Workspace)
                .Include(uwr => uwr.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (userWorkspaceRole == null)
            {
                throw new KeyNotFoundException("Workspace was not found, or you do not have access to it.");
            }

            var hasPermission = userWorkspaceRole.Role.RolePermissions
                .Any(rp => rp.Permission.Code == permissionCode);

            if (!hasPermission)
            {
                throw new ForbiddenException($"You do not have the '{permissionCode}' permission required for this operation.");
            }

            return userWorkspaceRole;
        }

        // ====================================================================================================
        // Queries
        // List projects in a workspace, and get one project by id.
        // ====================================================================================================

        public async Task<List<ProjectSummaryDto>> GetWorkspaceProjectsAsync(Guid userId, Guid workspaceId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Projects.Read);

            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.WorkspaceId == workspaceId)
                .OrderBy(p => p.CreatedAt)
                .Select(p => new ProjectSummaryDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Key = p.Key,
                    CreatedAt = p.CreatedAt,
                    Description = p.Description,
                    WorkspaceId = p.WorkspaceId
                }).ToListAsync();
        }

        public async Task<ProjectSummaryDto> GetProjectByIdAsync(Guid userId, Guid workspaceId, Guid projectId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Projects.Read);

            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Id == projectId);
  
            if (project == null)
            {
                throw new KeyNotFoundException("The project was not found in this workspace.");
            }

            return new ProjectSummaryDto
            {
                Id = project.Id,
                WorkspaceId = project.WorkspaceId,
                Name = project.Name,
                Key = project.Key,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }


        // ====================================================================================================
        // Commands
        // Create, update, and delete a project in a workspace.
        // ====================================================================================================

        public async Task<ProjectSummaryDto> CreateProjectAsync(Guid userId, Guid workspaceId, CreateProjectRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Projects.Create);

            var projectNameExists = await _context.Projects
                .AnyAsync(p => p.WorkspaceId == workspaceId && p.Name.ToLower() == request.Name.ToLower());

            if (projectNameExists)
            {
                throw new ConflictException($"A project named '{request.Name}' already exists in this workspace.");
            }

            var projectKeyExists = await _context.Projects
                .AnyAsync(p => p.WorkspaceId == workspaceId && p.Key != null && p.Key.ToLower() == request.Key.ToLower());

            if (projectKeyExists)
            {
                throw new ConflictException($"A project key '{request.Key}' already exists in this workspace.");
            }

            var newProject = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Key = request.Key,
                Description = request.Description,
                WorkspaceId = workspaceId,
                CreatedAt = DateTime.UtcNow,
                Workspace = null!
            };

            await _context.Projects.AddAsync(newProject);
            await _context.SaveChangesAsync();

            return new ProjectSummaryDto
            {
                Id = newProject.Id,
                Name = newProject.Name,
                Key = newProject.Key,
                Description = newProject.Description,
                WorkspaceId = newProject.WorkspaceId,
                CreatedAt = newProject.CreatedAt
            };
        }

        public async Task<ProjectSummaryDto> UpdateProjectAsync(Guid userId, Guid workspaceId, Guid projectId, UpdateProjectRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Projects.Update);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException("The project was not found in this workspace.");

            var isNameExists = await _context.Projects
                .AnyAsync(p =>
                    p.WorkspaceId == workspaceId &&      
                    p.Id != projectId &&                 
                    p.Name.ToLower() == request.Name.ToLower());

            if (isNameExists)  
                throw new ConflictException($"A project named '{request.Name}' already exists in this workspace.");

            var projectKeyExists = await _context.Projects
                .AnyAsync(p => p.WorkspaceId == workspaceId && p.Key != null && p.Key.ToLower() == request.Key.ToLower() && p.Id != projectId);

            if (projectKeyExists)
            {
                throw new ConflictException($"A project key '{request.Key}' already exists in this workspace.");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.Key = request.Key;

            await _context.SaveChangesAsync();

            return new ProjectSummaryDto
            {
                Id = project.Id,
                Name = project.Name,
                Key = project.Key,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                WorkspaceId = project.WorkspaceId
            };
        }

        public async Task<bool> DeleteProjectAsync(Guid userId, Guid workspaceId, Guid projectId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Projects.Delete);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException("The project was not found in this workspace.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return true;

        }
    }
}
