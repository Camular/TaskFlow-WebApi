using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Workspace;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;
using static TaskFlow.WebApi.Core.Authorization.SystemPermissions;
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

        // ====================================================================================================
        // Guards
        // Shared membership and permission check used by the actions below.
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
        // Workspaces
        // List, get, create, update, and delete a workspace.
        // ====================================================================================================

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

        public async Task<WorkspaceSummaryDto> GetWorkspaceByIdAsync(Guid userId, Guid workspaceId)
        {
            var userWorkspaceRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.Workspace)
                .Include(uwr => uwr.Role)
                .FirstOrDefaultAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (userWorkspaceRole == null)
            {
                throw new KeyNotFoundException("Workspace was not found, or you do not have access to it.");
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

        public async Task<WorkspaceSummaryDto> CreateWorkspaceAsync(Guid userId, CreateWorkspaceRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                bool isNameExists = await _context.UserWorkspaceRoles
                    .AnyAsync(uwr => uwr.UserId == userId && uwr.Workspace.SpaceName.ToLower() == request.SpaceName.ToLower());

                if (isNameExists)
                {
                    throw new InvalidOperationException($"You already have a workspace named '{request.SpaceName}'.");
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
                throw new InvalidOperationException("Personal workspaces cannot be deleted.");
            }

            _context.Workspaces.Remove(userWorkspaceRole.Workspace);
            await _context.SaveChangesAsync();

            return true;
        }


        // ====================================================================================================
        // Members
        // List members, change a member role, and remove a member.
        // ====================================================================================================

        public async Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(Guid userId, Guid workspaceId)
        {
            var isMember = await _context.UserWorkspaceRoles
                .AnyAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (!isMember)
            {
                throw new ForbiddenException("You must be a member of this workspace to view its members.");
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

        public async Task<WorkspaceMemberDto> UpdateMemberRoleAsync(Guid requesterUserId, Guid workspaceId, Guid targetUserId, UpdateMemberRoleRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(requesterUserId, workspaceId, SystemPermissions.Workspace.RoleAssign);

            var targetMemberRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.User)
                .FirstOrDefaultAsync(uwr => uwr.UserId == targetUserId && uwr.WorkspaceId == workspaceId);

            if (targetMemberRole == null)
            {
                throw new KeyNotFoundException("The member whose role you want to change was not found in this workspace.");
            }

            if (targetMemberRole.RoleId == SystemRoles.OwnerId && request.RoleId != SystemRoles.OwnerId)
            {
                var ownerCount = await _context.UserWorkspaceRoles
                    .CountAsync(uwr => uwr.WorkspaceId == workspaceId && uwr.RoleId == SystemRoles.OwnerId);

                if (ownerCount <= 1)
                {
                    throw new InvalidOperationException("A workspace must keep at least one Owner. You cannot change the role of the last Owner.");
                }
            }

            var newRole = await _context.WorkspaceRoles.FindAsync(request.RoleId);

            if (newRole == null || (newRole.WorkspaceId != null && newRole.WorkspaceId != workspaceId))
            {
                throw new KeyNotFoundException("The selected role was not found, or it is not valid for this workspace.");
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
                throw new KeyNotFoundException("The member you want to remove was not found in this workspace.");
            }

            if (targetMemberRole.RoleId == SystemRoles.OwnerId)
            {
                var ownerCount = await _context.UserWorkspaceRoles
                    .CountAsync(uwr => uwr.WorkspaceId == workspaceId && uwr.RoleId == SystemRoles.OwnerId);

                if (ownerCount <= 1)
                {
                    throw new InvalidOperationException("The only Owner of the workspace cannot be removed. Transfer ownership to another member first.");
                }
            }

            _context.UserWorkspaceRoles.Remove(targetMemberRole);
            await _context.SaveChangesAsync();

            return true;
        }


        // ====================================================================================================
        // Roles
        // List, get, create, update, and delete custom workspace roles.
        // ====================================================================================================

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
                throw new KeyNotFoundException("This role does not belong to your workspace.");
            }

            return workspaceRole;

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
                throw new InvalidOperationException("The selected permissions contain duplicates.");
            }

            if (roleNameExists)
            {
                throw new ConflictException($"A role named '{request.Name}' already exists.");
            }

            if (permissionCount != request.PermissionIds.Count)
            {
                throw new InvalidOperationException("One or more selected permissions are invalid or were not found.");
            }

            if(containsRestrictedPermissions)
            {
                throw new InvalidOperationException("The selected permissions include a permission that cannot be assigned.");
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
                throw new InvalidOperationException("The selected permissions contain duplicates.");
            }

            if (roleNameExists)
            {
                throw new ConflictException($"A role named '{request.Name}' already exists.");
            }

            if (permissionCount != request.PermissionIds.Count)
            {
                throw new InvalidOperationException("One or more selected permissions are invalid or were not found.");
            }

            if (containsRestrictedPermissions)
            {
                throw new InvalidOperationException("The selected permissions include a permission that cannot be assigned.");
            }

            var updateRole = await _context.WorkspaceRoles
                .Include(r => r.UserWorkspaceRoles)
                    .ThenInclude(uwr => uwr.User)
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == roleId && r.WorkspaceId == workspaceId);

            if(updateRole == null)
            {
                throw new KeyNotFoundException("The role you want to update was not found, or it does not belong to this workspace.");
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
                throw new KeyNotFoundException("The role you want to delete is a system role, or it does not belong to this workspace.");
            }

            if(roleInfo.UserWorkspaceRoles.Any())
            {
                throw new InvalidOperationException("This role cannot be deleted while members are still assigned to it.");
            }

            _context.WorkspaceRoles.Remove(roleInfo);
            await _context.SaveChangesAsync();

            return true;
        }


        // ====================================================================================================
        // Invitations
        // List, create, cancel, and accept workspace invitations.
        // ====================================================================================================

        public async Task<List<InvitationSummaryDto>> GetWorkspaceInvitationsAsync(Guid userId, Guid workspaceId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.MemberAdd);

            var invitationList = await _context.Invitations
                .Where(i => i.WorkspaceId == workspaceId)
                .Select(i => new InvitationSummaryDto
                {
                    Id = i.Id,
                    Email = i.Email,
                    RoleName = i.Role.Name,
                    RoleId = i.RoleId,
                    Status = i.Status,
                    Token = i.Token,
                    ExpiresAt = i.ExpiresAt,
                    WorkspaceId = i.WorkspaceId

                }).ToListAsync();

            return invitationList;
        }

        public async Task<InvitationSummaryDto> CreateInvitationAsync(Guid userId, Guid workspaceId, CreateInvitationRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.MemberAdd);

            string email = request.Email.Trim().ToLowerInvariant();

            var assignedRole = await _context.WorkspaceRoles.FindAsync(request.RoleId);

            if (assignedRole == null || (assignedRole.WorkspaceId != null && assignedRole.WorkspaceId != workspaceId))
            {
                throw new KeyNotFoundException("The selected role was not found, or it is not valid for this workspace.");
            }

            var targetUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if(targetUser != null)
            {
                var isAlreadyMember = await _context.UserWorkspaceRoles
                .AnyAsync(uwr => uwr.UserId == targetUser.Id && uwr.WorkspaceId == workspaceId);

                if (isAlreadyMember)
                {
                    throw new InvalidOperationException("This user is already a member of the workspace.");
                }
            }

            var isAlreadyInvited = await _context.Invitations
               .AnyAsync(i => i.Email == email && i.WorkspaceId == workspaceId && i.Status == InvitationStatus.Pending);
            
            if(isAlreadyInvited)
            {
                throw new ConflictException("A pending invitation already exists for this email address.");
            }

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = email,
                RoleId = request.RoleId,
                Status = InvitationStatus.Pending,
                WorkspaceId = workspaceId,
                InvitedByUserId = userId,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                InvitedBy = null!,
                Role = null!,
                Workspace = null!
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            return new InvitationSummaryDto
            {
                Id = invitation.Id,
                Email = invitation.Email,
                RoleId = invitation.RoleId,
                RoleName = assignedRole.Name,
                Token = invitation.Token,
                WorkspaceId = invitation.WorkspaceId,
                Status = invitation.Status,
                ExpiresAt = invitation.ExpiresAt
            };
        }

        public async Task<bool> CancelWorkspaceInvitationAsync(Guid userId, Guid workspaceId, Guid invitationId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.Workspace.MemberAdd);

            var invitation = await _context.Invitations
                .Where(i => i.Id == invitationId && i.WorkspaceId == workspaceId)
                .FirstOrDefaultAsync();

            if (invitation == null)
                throw new KeyNotFoundException("The invitation was not found, or it does not belong to this workspace.");

            if (invitation.Status != InvitationStatus.Pending)
                throw new InvalidOperationException("Only a pending invitation can be cancelled.");

            invitation.Status = InvitationStatus.Cancelled;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<WorkspaceMemberDto> AcceptInvitationAsync(Guid userId, AcceptInvitationRequest request)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new KeyNotFoundException("User was not found.");

            var invitation = await _context.Invitations
                .Include(i => i.Role)
                .Where(i => i.Token == request.Token.Trim())
                .FirstOrDefaultAsync();

            if (invitation == null)
                throw new KeyNotFoundException("Invitation was not found.");

            if (invitation.Status != InvitationStatus.Pending)
                throw new InvalidOperationException("A cancelled or already accepted invitation cannot be accepted again.");

            if(invitation.ExpiresAt < DateTime.UtcNow)
            {
                invitation.Status = InvitationStatus.Expired;
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("This invitation has expired and can no longer be accepted.");
            }

            if (invitation.Email.Trim().ToLowerInvariant() != user.Email.Trim().ToLowerInvariant())
                throw new ForbiddenException("This invitation was sent to a different email address than the signed-in account.");

            var isMember = await _context.UserWorkspaceRoles
                .Where(uwr => uwr.UserId == userId && uwr.WorkspaceId == invitation.WorkspaceId)
                .AnyAsync();

            if (isMember)
                throw new InvalidOperationException("You are already a member of this workspace.");

            var userWorkspaceRole = new UserWorkspaceRole
            {
                RoleId = invitation.RoleId,
                UserId = user.Id,
                WorkspaceId = invitation.WorkspaceId,
                Role = null!,
                User = null!,
                Workspace = null!
            };

            invitation.Status = InvitationStatus.Accepted;

            await _context.UserWorkspaceRoles.AddAsync(userWorkspaceRole);
            await _context.SaveChangesAsync();

            return new WorkspaceMemberDto
            {
                UserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                RoleId = invitation.RoleId,
                RoleName = invitation.Role.Name
            };
        }

    }
}