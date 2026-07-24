using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Workspace;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;
using Workspace = TaskFlow.WebApi.Core.Entities.Workspace;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly AppDbContext _context;

        public WorkspaceService(AppDbContext context)
        {
            _context = context;
        }

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
                throw new KeyNotFoundException("Çalışma alanı bulunamadı veya bu çalışma alanına erişim izniniz yok.");
            }

            var hasPermission = userWorkspaceRole.Role.RolePermissions
                .Any(rp => rp.Permission.Code == permissionCode);

            if (!hasPermission)
            {
                throw new ForbiddenException($"Bu işlemi gerçekleştirmek için '{permissionCode}' yetkiniz bulunmamaktadır.");
            }

            return userWorkspaceRole;
        }

        public async Task<List<WorkspaceSummaryDto>> GetUserWorkspaceSummaryAsync(Guid userId)
        {
            return await _context.UserWorkspaceRoles
                .Where(uwr => uwr.UserId == userId)
                .Select(uwr => new WorkspaceSummaryDto
                {
                    Id = uwr.Workspace.Id,
                    SpaceName = uwr.Workspace.SpaceName,
                    SpaceDescription = uwr.Workspace.SpaceDescription ?? "",
                    RoleName = uwr.Role.Name,
                    IsPersonal = uwr.Workspace.IsPersonal
                })
                .ToListAsync();
        }

        public async Task<WorkspaceSummaryDto> CreateWorkspaceAsync(Guid UserId, CreateWorkspaceRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var workspace = new Workspace
                {
                    Id = Guid.NewGuid(),
                    SpaceName = request.SpaceName,
                    SpaceDescription = request.SpaceDescription,
                    IsPersonal = false,
                    CreatedAt = DateTime.UtcNow
                };

                var userWorkspaceRole = new UserWorkspaceRole
                {
                    UserId = UserId,
                    WorkspaceId = workspace.Id,
                    RoleId = SystemRoles.OwnerId,
                    Role = null!,
                    User = null!,
                    Workspace = workspace
                };

                _context.Workspaces.Add(workspace);
                _context.UserWorkspaceRoles.Add(userWorkspaceRole);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new WorkspaceSummaryDto
                {
                    Id = workspace.Id,
                    SpaceName = workspace.SpaceName,
                    SpaceDescription = workspace.SpaceDescription ?? "",
                    IsPersonal = workspace.IsPersonal,
                    RoleName = "Owner"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WorkspaceSummaryDto?> UpdateWorkspaceAsync(Guid userId, Guid workspaceId, UpdateWorkspaceRequest request)
        {
            var userWorkspaceRole = await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.Update);

            userWorkspaceRole.Workspace.SpaceName = request.SpaceName;
            userWorkspaceRole.Workspace.SpaceDescription = request.SpaceDescription;

            await _context.SaveChangesAsync();

            return new WorkspaceSummaryDto
            {
                Id = userWorkspaceRole.Workspace.Id,
                SpaceName = userWorkspaceRole.Workspace.SpaceName,
                SpaceDescription = userWorkspaceRole.Workspace.SpaceDescription ?? "",
                IsPersonal = userWorkspaceRole.Workspace.IsPersonal,
                RoleName = userWorkspaceRole.Role.Name
            };
        }

        public async Task<bool> DeleteWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var userWorkspaceRole = await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.Delete);

            if (userWorkspaceRole.Workspace.IsPersonal)
            {
                throw new InvalidOperationException("Kişisel çalışma alanları silinemez.");
            }

            _context.Workspaces.Remove(userWorkspaceRole.Workspace);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WorkspaceSummaryDto?> GetWorkspaceByIdAsync(Guid userId, Guid workspaceId)
        {
            var userWorkspaceRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.Workspace)
                .Include(uwr => uwr.Role)
                .FirstOrDefaultAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (userWorkspaceRole == null)
            {
                throw new KeyNotFoundException("Çalışma alanı bulunamadı veya bu çalışma alanına erişim izniniz yok.");
            }

            return new WorkspaceSummaryDto
            {
                Id = userWorkspaceRole.Workspace.Id,
                SpaceName = userWorkspaceRole.Workspace.SpaceName,
                SpaceDescription = userWorkspaceRole.Workspace.SpaceDescription ?? "",
                IsPersonal = userWorkspaceRole.Workspace.IsPersonal,
                RoleName = userWorkspaceRole.Role.Name
            };
        }

        public async Task<List<WorkspaceMemberDto>?> GetWorkspaceMembersAsync(Guid userId, Guid workspaceId)
        {
            var isMember = await _context.UserWorkspaceRoles
                .AnyAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new InvalidOperationException("Bu çalışma alanının üyelerini görüntülemek için çalışma alanına üye olmalısınız.");
            }

            return await _context.UserWorkspaceRoles
                .Where(uwr => uwr.WorkspaceId == workspaceId)
                .Select(uwr => new WorkspaceMemberDto
                {
                    UserId = uwr.UserId,
                    RoleId = uwr.RoleId,
                    Email = uwr.User.Email,
                    RoleName = uwr.Role.Name,
                    UserName = uwr.User.Username
                })
                .ToListAsync();
        }

        public async Task<WorkspaceMemberDto?> AddWorkspaceMemberAsync(Guid requesterUserId, Guid workspaceId, AddWorkspaceMemberRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(requesterUserId, workspaceId, SystemPermissions.Workspace.MemberAdd);

            var targetUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant());

            if (targetUser == null)
            {
                throw new KeyNotFoundException("Eklenmek istenen e-posta adresine sahip bir kullanıcı bulunamadı.");
            }

            var isAlreadyMember = await _context.UserWorkspaceRoles
                .AnyAsync(uwr => uwr.UserId == targetUser.Id && uwr.WorkspaceId == workspaceId);

            if (isAlreadyMember)
            {
                throw new InvalidOperationException("Bu kullanıcı zaten bu çalışma alanının bir üyesidir.");
            }

            var assignedRole = await _context.WorkspaceRoles.FindAsync(request.RoleId);

            if (assignedRole == null || (assignedRole.WorkspaceId != null && assignedRole.WorkspaceId != workspaceId))
            {
                throw new KeyNotFoundException("Atanmak istenen rol bulunamadı veya bu çalışma alanı için geçerli değil.");
            }

            var userWorkspaceRole = new UserWorkspaceRole
            {
                RoleId = request.RoleId,
                UserId = targetUser.Id,
                WorkspaceId = workspaceId,
                Role = null!,
                User = null!,
                Workspace = null!
            };

            await _context.UserWorkspaceRoles.AddAsync(userWorkspaceRole);
            await _context.SaveChangesAsync();

            return new WorkspaceMemberDto
            {
                UserId = targetUser.Id,
                UserName = targetUser.Username,
                Email = targetUser.Email,
                RoleId = request.RoleId,
                RoleName = assignedRole.Name
            };
        }

        public async Task<WorkspaceMemberDto?> UpdateMemberRoleAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId, UpdateMemberRoleRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(requesterUserId, workspaceId, SystemPermissions.Workspace.RoleAssign);

            var targetMemberRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.User)
                .FirstOrDefaultAsync(uwr => uwr.UserId == targetUserId && uwr.WorkspaceId == workspaceId);

            if (targetMemberRole == null)
            {
                throw new KeyNotFoundException("Rolü değiştirilmek istenen üye bu çalışma alanında bulunamadı.");
            }

            if (targetMemberRole.RoleId == SystemRoles.OwnerId && request.RoleId != SystemRoles.OwnerId)
            {
                var ownerCount = await _context.UserWorkspaceRoles
                    .CountAsync(uwr => uwr.WorkspaceId == workspaceId && uwr.RoleId == SystemRoles.OwnerId);

                if (ownerCount <= 1)
                {
                    throw new InvalidOperationException("Çalışma alanında en az bir Sahip (Owner) bulunmalıdır. Son sahibin rolünü değiştiremezsiniz.");
                }
            }

            var newRole = await _context.WorkspaceRoles.FindAsync(request.RoleId);

            if (newRole == null || (newRole.WorkspaceId != null && newRole.WorkspaceId != workspaceId))
            {
                throw new KeyNotFoundException("Atanmak istenen rol bulunamadı veya bu çalışma alanı için geçerli değil.");
            }

            _context.UserWorkspaceRoles.Remove(targetMemberRole);

            var newUserWorkspaceRole = new UserWorkspaceRole
            {
                UserId = targetUserId,
                WorkspaceId = workspaceId,
                RoleId = request.RoleId,
                Role = null!,
                User = null!,
                Workspace = null!
            };

            await _context.UserWorkspaceRoles.AddAsync(newUserWorkspaceRole);
            await _context.SaveChangesAsync();

            return new WorkspaceMemberDto
            {
                UserId = targetMemberRole.UserId,
                UserName = targetMemberRole.User.Username,
                Email = targetMemberRole.User.Email,
                RoleId = newRole.Id,
                RoleName = newRole.Name
            };
        }

        public async Task<bool> RemoveWorkspaceMemberAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId)
        {
            await CheckAndGetWorkspaceRoleAsync(requesterUserId, workspaceId, SystemPermissions.Workspace.MemberRemove);

            var targetMemberRole = await _context.UserWorkspaceRoles
                .FirstOrDefaultAsync(uwr => uwr.UserId == targetUserId && uwr.WorkspaceId == workspaceId);

            if (targetMemberRole == null)
            {
                throw new KeyNotFoundException("Çıkarılmak istenen üye bu çalışma alanında bulunamadı.");
            }

            if (targetMemberRole.RoleId == SystemRoles.OwnerId)
            {
                var ownerCount = await _context.UserWorkspaceRoles
                    .CountAsync(uwr => uwr.WorkspaceId == workspaceId && uwr.RoleId == SystemRoles.OwnerId);

                if (ownerCount <= 1)
                {
                    throw new InvalidOperationException("Çalışma alanındaki tek Sahip (Owner) takımdan çıkarılamaz. Önce sahipliği başka bir üyeye devretmelisiniz.");
                }
            }

            _context.UserWorkspaceRoles.Remove(targetMemberRole);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}