using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.Entities;


namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(

                // ====================================================================================================
                // 1. OWNER — full access
                // ====================================================================================================

                // 1.1 Workspace permissions (full)
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.ViewId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.MemberAddId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.MemberRemoveId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.RoleAssignId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.RoleCreateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.RoleUpdateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.UpdateId },       
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.DeleteId },      
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Workspace.RoleDeleteId },   

                // 1.2 Task permissions (full)
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.CreateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.ReadId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.UpdateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.DeleteId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.StatusUpdateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.DeadlineUpdateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.TaskItem.AssignId },

                // 1.3 Project permissions (full)
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Projects.CreateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Projects.ReadId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Projects.UpdateId },       
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Projects.DeleteId },      

                // 1.4 Comment permissions (full)
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Comments.CreateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Comments.ReadId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Comments.UpdateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Comments.DeleteId },

                // 1.5 Attachment permissions (full)
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Attachments.CreateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Attachments.ReadId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Attachments.UpdateId },
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.Attachments.DeleteId },

                // 1.6 Activity log permissions
                new RolePermission { RoleId = SystemRoles.OwnerId, PermissionId = SystemPermissions.ActivityLog.ReadId },


                // ====================================================================================================
                // 2. ADMINISTRATOR — full access except deleting the workspace
                // ====================================================================================================

                // 2.1 Workspace permissions (members, settings, custom roles; no workspace delete)
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.ViewId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.MemberAddId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.MemberRemoveId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.RoleAssignId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.UpdateId },       
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.RoleDeleteId },   
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.RoleCreateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Workspace.RoleUpdateId },

                // 2.2 Task permissions (full)
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.CreateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.ReadId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.UpdateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.DeleteId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.StatusUpdateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.DeadlineUpdateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.TaskItem.AssignId },

                // 2.3 Project permissions (full)
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Projects.CreateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Projects.ReadId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Projects.UpdateId },       
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Projects.DeleteId },       

                // 2.4 Comment permissions (full)
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Comments.CreateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Comments.ReadId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Comments.UpdateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Comments.DeleteId },

                // 2.5 Attachment permissions (full)
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Attachments.CreateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Attachments.ReadId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Attachments.UpdateId },
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.Attachments.DeleteId },

                // 2.6 Activity log permissions
                new RolePermission { RoleId = SystemRoles.AdministratorId, PermissionId = SystemPermissions.ActivityLog.ReadId },


                // ====================================================================================================
                // 3. MEMBER — day-to-day work, without delete or workspace settings
                // ====================================================================================================

                // 3.1 Workspace permissions (view only)
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Workspace.ViewId },

                // 3.2 Task permissions (create, update, assign; no delete or deadline change)
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.TaskItem.CreateId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.TaskItem.ReadId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.TaskItem.UpdateId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.TaskItem.StatusUpdateId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.TaskItem.AssignId },

                // 3.3 Project permissions (view only)
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Projects.ReadId },

                // 3.4 Comment permissions (create, read, update; no delete)
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Comments.CreateId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Comments.ReadId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Comments.UpdateId },

                // 3.5 Attachment permissions (create, read, update; no delete)
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Attachments.CreateId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Attachments.ReadId },
                new RolePermission { RoleId = SystemRoles.MemberId, PermissionId = SystemPermissions.Attachments.UpdateId },


                // ====================================================================================================
                // 4. VIEWER — read only
                // ====================================================================================================

                // 4.1 Workspace permissions (view only)
                new RolePermission { RoleId = SystemRoles.ViewerId, PermissionId = SystemPermissions.Workspace.ViewId },

                // 4.2 Task permissions (view only)
                new RolePermission { RoleId = SystemRoles.ViewerId, PermissionId = SystemPermissions.TaskItem.ReadId },

                // 4.3 Project permissions (view only)
                new RolePermission { RoleId = SystemRoles.ViewerId, PermissionId = SystemPermissions.Projects.ReadId },

                // 4.4 Comment permissions (view only)
                new RolePermission { RoleId = SystemRoles.ViewerId, PermissionId = SystemPermissions.Comments.ReadId },

                // 4.5 Attachment permissions (view only)
                new RolePermission { RoleId = SystemRoles.ViewerId, PermissionId = SystemPermissions.Attachments.ReadId }

            );
        }
        
    }
}
