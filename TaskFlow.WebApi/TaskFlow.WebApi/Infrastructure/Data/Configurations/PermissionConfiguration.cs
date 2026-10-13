using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.Entities;

namespace TaskFlow.WebApi.Infrastructure.Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code).HasMaxLength(100).IsRequired();

            builder.Property(p => p.Description).HasMaxLength(250).IsRequired();

            builder.HasIndex(p => p.Code)
                .IsUnique();

            builder.HasData(

                // ==========================================
                //              Task permissions
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.TaskItem.CreateId,
                    Code = SystemPermissions.TaskItem.Create,
                    Description = "Create tasks in the workspace."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.ReadId,
                    Code = SystemPermissions.TaskItem.Read,
                    Description = "View tasks in the workspace."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.UpdateId,
                    Code = SystemPermissions.TaskItem.Update,
                    Description = "Update tasks in the workspace."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.DeleteId,
                    Code = SystemPermissions.TaskItem.Delete,
                    Description = "Delete tasks in the workspace."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.StatusUpdateId,
                    Code = SystemPermissions.TaskItem.StatusUpdate,
                    Description = "Update the status of tasks in the workspace."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.DeadlineUpdateId,
                    Code = SystemPermissions.TaskItem.DeadlineUpdate,
                    Description = "Update task deadlines in the workspace."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.AssignId,
                    Code = SystemPermissions.TaskItem.Assign,
                    Description = "Assign tasks in the workspace."
                },

                // ==========================================
                //              Workspace permissions
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Workspace.ViewId,
                    Code = SystemPermissions.Workspace.View,
                    Description = "View the workspace and its summary."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.MemberAddId,
                    Code = SystemPermissions.Workspace.MemberAdd,
                    Description = "Invite a member to the workspace."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.MemberRemoveId,
                    Code = SystemPermissions.Workspace.MemberRemove,
                    Description = "Remove a member from the workspace."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleAssignId,
                    Code = SystemPermissions.Workspace.RoleAssign,
                    Description = "Assign and change member roles in the workspace."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleCreateId,
                    Code = SystemPermissions.Workspace.RoleCreate,
                    Description = "Create custom roles for the workspace."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleUpdateId,
                    Code = SystemPermissions.Workspace.RoleUpdate,
                    Description = "Update the permission set of a custom workspace role."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleDeleteId,
                    Code = SystemPermissions.Workspace.RoleDelete,
                    Description = "Delete a custom role from the workspace."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.UpdateId,
                    Code = SystemPermissions.Workspace.Update,
                    Description = "Update the workspace name, description, and settings."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.DeleteId,
                    Code = SystemPermissions.Workspace.Delete,
                    Description = "Permanently delete the workspace and its data."
                },


                // ==========================================
                //              Activity log permissions
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.ActivityLog.ReadId,
                    Code = SystemPermissions.ActivityLog.Read,
                    Description = "View activity logs."
                },

                // ==========================================
                //              Comment permissions
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Comments.CreateId,
                    Code = SystemPermissions.Comments.Create,
                    Description = "Create comments on tasks."
                },

                new Permission
                {
                    Id = SystemPermissions.Comments.ReadId,
                    Code = SystemPermissions.Comments.Read,
                    Description = "View comments on tasks."
                },

                new Permission
                {
                    Id = SystemPermissions.Comments.UpdateId,
                    Code = SystemPermissions.Comments.Update,
                    Description = "Update comments on tasks."
                },

                new Permission
                {
                    Id = SystemPermissions.Comments.DeleteId,
                    Code = SystemPermissions.Comments.Delete,
                    Description = "Delete comments on tasks."
                },

                // ==========================================
                //              Project permissions
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Projects.CreateId,
                    Code = SystemPermissions.Projects.Create,
                    Description = "Create projects in the workspace."
                },
                new Permission
                {
                    Id = SystemPermissions.Projects.ReadId,
                    Code = SystemPermissions.Projects.Read,
                    Description = "View projects and their details."
                },
                new Permission
                {
                    Id = SystemPermissions.Projects.UpdateId,
                    Code = SystemPermissions.Projects.Update,
                    Description = "Update a project's name, color, category, and settings."
                },
                new Permission
                {
                    Id = SystemPermissions.Projects.DeleteId,
                    Code = SystemPermissions.Projects.Delete,
                    Description = "Permanently delete a project and its tasks."
                },

                // ==========================================
                //              Attachment permissions
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Attachments.CreateId,
                    Code = SystemPermissions.Attachments.Create,
                    Description = "Add files to tasks."
                },

                new Permission
                {
                    Id = SystemPermissions.Attachments.ReadId,
                    Code = SystemPermissions.Attachments.Read,
                    Description = "View files attached to tasks."
                },

                new Permission
                {
                    Id = SystemPermissions.Attachments.DeleteId,
                    Code = SystemPermissions.Attachments.Delete,
                    Description = "Delete files attached to tasks."
                },

                new Permission
                {
                    Id = SystemPermissions.Attachments.UpdateId,
                    Code = SystemPermissions.Attachments.Update,
                    Description = "Update files attached to tasks."
                }
            );
        }
    }
}
