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
                //              Task İzinleri
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.TaskItem.CreateId,
                    Code = SystemPermissions.TaskItem.Create,
                    Description = "Grup içindeki görevleri oluşturma yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.ReadId,
                    Code = SystemPermissions.TaskItem.Read,
                    Description = "Grup içindeki görevleri görüntüleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.UpdateId,
                    Code = SystemPermissions.TaskItem.Update,
                    Description = "Grup içindeki görevleri güncelleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.DeleteId,
                    Code = SystemPermissions.TaskItem.Delete,
                    Description = "Grup içindeki görevleri silme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.StatusUpdateId,
                    Code = SystemPermissions.TaskItem.StatusUpdate,
                    Description = "Grup içindeki görevlerin durumunu güncelleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.DeadlineUpdateId,
                    Code = SystemPermissions.TaskItem.DeadlineUpdate,
                    Description = "Grup içindeki görevlerin son teslim tarihini güncelleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.TaskItem.AssignId,
                    Code = SystemPermissions.TaskItem.Assign,
                    Description = "Grup içindeki görevleri atama yetkisi."
                },

                // ==========================================
                //              Workspace İzinleri
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Workspace.ViewId,
                    Code = SystemPermissions.Workspace.View,
                    Description = "Çalışma alanını ve genel özet ekranını görüntüleme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.MemberAddId,
                    Code = SystemPermissions.Workspace.MemberAdd,
                    Description = "Çalışma alanına yeni takım arkadaşı/üye davet etme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.MemberRemoveId,
                    Code = SystemPermissions.Workspace.MemberRemove,
                    Description = "Çalışma alanından mevcut bir üyeyi tamamen çıkarma yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleAssignId,
                    Code = SystemPermissions.Workspace.RoleAssign,
                    Description = "Çalışma alanındaki üyelere rol (Admin, Member vb.) atama ve değiştirme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleCreateId,
                    Code = SystemPermissions.Workspace.RoleCreate,
                    Description = "Çalışma alanına özel (Custom Role) yeni roller tanımlama yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleUpdateId,
                    Code = SystemPermissions.Workspace.RoleUpdate,
                    Description = "Çalışma alanına özel tanımlanmış rollerin yetki matrisini güncelleme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.RoleDeleteId,
                    Code = SystemPermissions.Workspace.RoleDelete,
                    Description = "Çalışma alanına özel oluşturulmuş rolleri tamamen silme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.UpdateId,
                    Code = SystemPermissions.Workspace.Update,
                    Description = "Çalışma alanının ismini, açıklamasını ve kurumsal genel ayarlarını düzenleme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Workspace.DeleteId,
                    Code = SystemPermissions.Workspace.Delete,
                    Description = "Çalışma alanını (Workspace) tüm verileriyle birlikte kalıcı olarak silme yetkisi."
                },


                // ==========================================
                //              ActivityLog İzinleri
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.ActivityLog.ReadId,
                    Code = SystemPermissions.ActivityLog.Read,
                    Description = "Activity logları görüntüleme yetkisi."
                },

                // ==========================================
                //              Comments İzinleri
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Comments.CreateId,
                    Code = SystemPermissions.Comments.Create,
                    Description = "Görevler için yorum oluşturma yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.Comments.ReadId,
                    Code = SystemPermissions.Comments.Read,
                    Description = "Görevler için yorumları görüntüleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.Comments.UpdateId,
                    Code = SystemPermissions.Comments.Update,
                    Description = "Görevler için yorumları güncelleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.Comments.DeleteId,
                    Code = SystemPermissions.Comments.Delete,
                    Description = "Görevler için yorumları silme yetkisi."
                },

                // ==========================================
                //              Projects İzinleri
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Projects.CreateId,
                    Code = SystemPermissions.Projects.Create,
                    Description = "Çalışma alanı altında yeni panolar/projeler oluşturma yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Projects.ReadId,
                    Code = SystemPermissions.Projects.Read,
                    Description = "Mevcut projeleri, pano listelerini ve proje detaylarını görüntüleme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Projects.UpdateId,
                    Code = SystemPermissions.Projects.Update,
                    Description = "Mevcut projelerin isim, renk, kategori ve temel ayarlarını düzenleme yetkisi."
                },
                new Permission
                {
                    Id = SystemPermissions.Projects.DeleteId,
                    Code = SystemPermissions.Projects.Delete,
                    Description = "Bir projeyi/panoyu içindeki tüm görevlerle birlikte kalıcı olarak silme yetkisi."
                },

                // ==========================================
                //              Attachments İzinleri
                // ==========================================

                new Permission
                {
                    Id = SystemPermissions.Attachments.CreateId,
                    Code = SystemPermissions.Attachments.Create,
                    Description = "Görevler için dosya ekleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.Attachments.ReadId,
                    Code = SystemPermissions.Attachments.Read,
                    Description = "Görevler için dosyaları görüntüleme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.Attachments.DeleteId,
                    Code = SystemPermissions.Attachments.Delete,
                    Description = "Görevler için dosyaları silme yetkisi."
                },

                new Permission
                {
                    Id = SystemPermissions.Attachments.UpdateId,
                    Code = SystemPermissions.Attachments.Update,
                    Description = "Görevler için dosyaları güncelleme yetkisi."
                }
            );
        }
    }
}
