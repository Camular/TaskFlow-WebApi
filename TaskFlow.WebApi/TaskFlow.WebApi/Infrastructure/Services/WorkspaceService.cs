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

        public async Task<WorkspaceSummaryDto> CreateWorkspaceAsync(Guid userId, CreateWorkspaceRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                bool isNameExists = await _context.UserWorkspaceRoles
                    .AnyAsync(uwr => uwr.UserId == userId && uwr.Workspace.SpaceName.ToLower() == request.SpaceName.ToLower());

                if (isNameExists)
                {
                    throw new InvalidOperationException($"'{request.SpaceName}' adında bir çalışma alanınız zaten mevcut.");
                }

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
                    UserId = userId,
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

        public async Task<WorkspaceSummaryDto> UpdateWorkspaceAsync(Guid userId, Guid workspaceId, UpdateWorkspaceRequest request)
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

        public async Task<WorkspaceSummaryDto> GetWorkspaceByIdAsync(Guid userId, Guid workspaceId)
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

        public async Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(Guid userId, Guid workspaceId)
        {
            var isMember = await _context.UserWorkspaceRoles
                .AnyAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new ForbiddenException("Bu çalışma alanının üyelerini görüntülemek için çalışma alanına üye olmalısınız.");
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

        public async Task<List<RoleSummaryDto>> GetWorkspaceRolesSummaryAsync(Guid userId, Guid workspaceId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.View);

            return await _context.WorkspaceRoles
                .Where(wr => wr.WorkspaceId == workspaceId || wr.WorkspaceId == null)
                .Select(wr => new RoleSummaryDto
                {
                    RoleId = wr.Id,
                    WorkspaceId = wr.WorkspaceId,
                    Name = wr.Name,
                    Permissions = wr.RolePermissions.Select(rp => new PermissionSummaryDto
                    {

                        PermissionId = rp.PermissionId,
                        Code = rp.Permission.Code ?? ""
                    }).ToList(),

                    AssignedUsers = wr.UserWorkspaceRoles
                    .Where(uwr => uwr.WorkspaceId == workspaceId)
                    .Select(uwr => new AssignedUsersSummaryDto
                    {
                        Id = uwr.UserId,
                        Email = uwr.User.Email
                    })
                    .ToList(),

                }).ToListAsync();
            
        }
        
        
        public async Task<RoleSummaryDto> GetWorkspaceRoleByIdSummaryAsync(Guid userId, Guid workspaceId, Guid roleId)
        {

            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.View);

            var workspaceRole = await _context.WorkspaceRoles
                .Where(wr => (wr.WorkspaceId == workspaceId || wr.WorkspaceId == null) && (wr.Id == roleId))
                .Select(wr => new RoleSummaryDto
                {
                    RoleId = wr.Id,
                    WorkspaceId = wr.WorkspaceId,
                    Name = wr.Name,
                    Permissions = wr.RolePermissions.Select(rp => new PermissionSummaryDto
                    {

                        PermissionId = rp.PermissionId,
                        Code = rp.Permission.Code ?? ""
                    }).ToList(),

                    AssignedUsers = wr.UserWorkspaceRoles
                    .Where(uwr => uwr.WorkspaceId == workspaceId)
                    .Select(uwr => new AssignedUsersSummaryDto
                    {
                        Id = uwr.UserId,
                        Email = uwr.User.Email
                    })
                    .ToList(),

                }).FirstOrDefaultAsync();

            if (workspaceRole == null)
            {
                throw new KeyNotFoundException("Bu Role Bulunduğunuz Workspace'e ait değildir.");
            }

            return workspaceRole;

        }
        
        public async Task<WorkspaceMemberDto> AddWorkspaceMemberAsync(Guid requesterUserId, Guid workspaceId, AddWorkspaceMemberRequest request)
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

        public async Task<WorkspaceMemberDto> UpdateMemberRoleAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId, UpdateMemberRoleRequest request)
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

        public async Task<RoleSummaryDto> CreateWorkspaceRoleAsync(Guid userId, Guid workspaceId, CreateWorkspaceRoleRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.RoleCreate);

            var permissionCount = await _context.Permissions
                .CountAsync(p => request.PermissionIds.Contains(p.Id));

            var roleNameExists = await _context.WorkspaceRoles
                .AnyAsync(r => (r.WorkspaceId == null && r.Name.ToLower() == request.Name.ToLower()) || (r.WorkspaceId == workspaceId && r.Name.ToLower() == request.Name.ToLower()));

            var permissionIdDublicateCheck = request.PermissionIds.Distinct().Count() != request.PermissionIds.Count;

            var containsRestrictedPermissions = request.PermissionIds.Contains(SystemPermissions.Workspace.DeleteId);

            var permissionInfo = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync();

            if (permissionIdDublicateCheck)
            {
                throw new InvalidOperationException("Atanmak istenen izinler arasında tekrar eden izinler bulunuyor.");
            }

            if (roleNameExists)
            {
                throw new ConflictException($"'{request.Name}' adında bir rol zaten mevcut.");
            }

            if (permissionCount != request.PermissionIds.Count)
            {
                throw new InvalidOperationException("Atanmak istenen izinlerden bazıları geçersiz veya bulunamadı.");
            }

            if(containsRestrictedPermissions)
            {
                throw new InvalidOperationException("Atanmak istenen izinler arasında yasaklı izinler bulunuyor.");
            }

            var newRole = new WorkspaceRole
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                Name = request.Name,
                RolePermissions = request.PermissionIds.Select(pid => new RolePermission
                {
                    PermissionId = pid
                }).ToList(),
                UserWorkspaceRoles = new List<UserWorkspaceRole>(),
                Workspace = null!
            };

            await _context.WorkspaceRoles.AddAsync(newRole);
            await _context.SaveChangesAsync();

            return new RoleSummaryDto
            {
                RoleId = newRole.Id,
                WorkspaceId = newRole.WorkspaceId,
                Name = newRole.Name,
                Permissions = newRole.RolePermissions.Select(rp =>
                {
                    var info = permissionInfo.FirstOrDefault(p => p.Id == rp.PermissionId);
                    return new PermissionSummaryDto
                    {

                        PermissionId = rp.PermissionId,
                        Code = info?.Code ?? ""
                    };

                }).ToList(),
                AssignedUsers = new List<AssignedUsersSummaryDto>(),
            };
        }

        public async Task<RoleSummaryDto> UpdateWorkspaceRoleAsync(Guid userId, Guid workspaceId, Guid roleId, UpdateWorkspaceRoleRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.RoleUpdate);

            var permissionCount = await _context.Permissions
                .CountAsync(p => request.PermissionIds.Contains(p.Id));

            var roleNameExists = await _context.WorkspaceRoles
                 .AnyAsync(r => ((r.WorkspaceId == null && r.Name.ToLower() == request.Name.ToLower()) || (r.WorkspaceId == workspaceId && r.Name.ToLower() == request.Name.ToLower())) && (r.Id != roleId));

            var permissionIdDublicateCheck = request.PermissionIds.Distinct().Count() != request.PermissionIds.Count;

            var containsRestrictedPermissions = request.PermissionIds.Contains(SystemPermissions.Workspace.DeleteId);

            var permissionInfo = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync();

            if (permissionIdDublicateCheck)
            {
                throw new InvalidOperationException("Atanmak istenen izinler arasında tekrar eden izinler bulunuyor.");
            }

            if (roleNameExists)
            {
                throw new ConflictException($"'{request.Name}' adında bir rol zaten mevcut.");
            }

            if (permissionCount != request.PermissionIds.Count)
            {
                throw new InvalidOperationException("Atanmak istenen izinlerden bazıları geçersiz veya bulunamadı.");
            }

            if (containsRestrictedPermissions)
            {
                throw new InvalidOperationException("Atanmak istenen izinler arasında yasaklı izinler bulunuyor.");
            }

            var updateRole = await _context.WorkspaceRoles
                .Include(r => r.UserWorkspaceRoles)
                    .ThenInclude(uwr => uwr.User)
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == roleId && r.WorkspaceId == workspaceId);

            if(updateRole == null)
            {
                throw new KeyNotFoundException("Güncellenmek istenen role bulunamadı veya bu çalışma alanına ait değil.");
            }

            _context.RemoveRange(updateRole.RolePermissions);

            updateRole.Name = request.Name;

            updateRole.RolePermissions = request.PermissionIds.Select(pid => new RolePermission
            {
                PermissionId = pid
            }).ToList();

            await _context.SaveChangesAsync();

            return new RoleSummaryDto
            {
                Name = updateRole.Name,
                Permissions = updateRole.RolePermissions.Select(rp =>
                {
                    var info = permissionInfo.FirstOrDefault(p => p.Id == rp.PermissionId);
                    return new PermissionSummaryDto
                    {

                        PermissionId = rp.PermissionId,
                        Code = info?.Code ?? ""
                    };

                }).ToList(),
                RoleId = roleId,
                AssignedUsers = updateRole.UserWorkspaceRoles
                    .Where(uwr => uwr.WorkspaceId == workspaceId)
                    .Select(uwr => new AssignedUsersSummaryDto
                    {
                        Id = uwr.UserId,
                        Email = uwr.User.Email
                    })
                    .ToList(),
                WorkspaceId = updateRole.WorkspaceId
            };
        }

        public async Task<bool> DeleteWorkspaceRoleAsync(Guid userId, Guid workspaceId, Guid roleId)
        {
            var userWorkspaceRole = await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.RoleDelete);

            var roleInfo = await _context.WorkspaceRoles
                .Include(wr => wr.UserWorkspaceRoles)
                .FirstOrDefaultAsync(wr => wr.Id == roleId && wr.WorkspaceId == workspaceId);

            if(roleInfo == null)
            {
                throw new KeyNotFoundException("Silinmek istenen rol bir sistem rolü veya bu workspace de bulunmamaktadır.");
            }

            if(roleInfo.UserWorkspaceRoles.Any())
            {
                throw new InvalidOperationException("silinmek istenen role atanmış kişiler bulunmaktadır.");
            }

            _context.WorkspaceRoles.Remove(roleInfo);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}