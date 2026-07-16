using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskFlow.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpaceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SpaceDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPersonal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TaskItems_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceRoles_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_WorkspaceRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "WorkspaceRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkspaceRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkspaceRoles", x => new { x.UserId, x.RoleId, x.WorkspaceId });
                    table.ForeignKey(
                        name: "FK_UserWorkspaceRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkspaceRoles_WorkspaceRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "WorkspaceRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkspaceRoles_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("02b08e56-e99f-4ef3-a99e-8ec70754194f"), "workspace.member.add", "Çalışma alanına yeni takım arkadaşı/üye davet etme yetkisi." },
                    { new Guid("0592df3c-6b6b-4b1d-bff2-5d779755a890"), "workspace.role.update", "Çalışma alanına özel tanımlanmış rollerin yetki matrisini güncelleme yetkisi." },
                    { new Guid("07a67e69-1d76-4ba4-a644-f04863a29c90"), "taskitem.read", "Grup içindeki görevleri görüntüleme yetkisi." },
                    { new Guid("11afb428-b2d3-40d3-8008-b5c7aecba8b4"), "comments.read", "Görevler için yorumları görüntüleme yetkisi." },
                    { new Guid("1a7b0835-2203-4305-b9bc-17633440d35c"), "workspace.delete", "Çalışma alanını (Workspace) tüm verileriyle birlikte kalıcı olarak silme yetkisi." },
                    { new Guid("23c879f0-49ca-402a-9610-574677f4f70d"), "activitylog.read", "Activity logları görüntüleme yetkisi." },
                    { new Guid("31e13039-cb2f-497c-8fb9-1d98d480aa96"), "comments.delete", "Görevler için yorumları silme yetkisi." },
                    { new Guid("365ccfa9-d0ce-4a7a-8e07-a7eaa4238474"), "taskitem.status.update", "Grup içindeki görevlerin durumunu güncelleme yetkisi." },
                    { new Guid("5193bc28-ea67-43d7-a105-7026d2d668e3"), "taskitem.assign", "Grup içindeki görevleri atama yetkisi." },
                    { new Guid("601059f0-0212-498f-9bf4-cdee28e4ba2c"), "workspace.role.assign", "Çalışma alanındaki üyelere rol (Admin, Member vb.) atama ve değiştirme yetkisi." },
                    { new Guid("6151859b-1c5c-4bd3-8202-656c02ae189d"), "projects.create", "Çalışma alanı altında yeni panolar/projeler oluşturma yetkisi." },
                    { new Guid("66f5f8bc-a5cf-4851-8c01-79dd50cfa4b6"), "projects.read", "Mevcut projeleri, pano listelerini ve proje detaylarını görüntüleme yetkisi." },
                    { new Guid("6b08ed62-b54e-41b5-8f10-0e52062150df"), "workspace.role.create", "Çalışma alanına özel (Custom Role) yeni roller tanımlama yetkisi." },
                    { new Guid("753469fe-c14a-4e8a-914e-786f3e78322d"), "taskitem.delete", "Grup içindeki görevleri silme yetkisi." },
                    { new Guid("8c9a2c14-6516-493e-9b21-56c113a86c13"), "workspace.view", "Çalışma alanını ve genel özet ekranını görüntüleme yetkisi." },
                    { new Guid("912c74bc-4f5c-4f1c-8753-05cfad084bfd"), "taskitem.deadline.update", "Grup içindeki görevlerin son teslim tarihini güncelleme yetkisi." },
                    { new Guid("93ecadf9-ce30-4843-ad61-236d39e7184b"), "comments.create", "Görevler için yorum oluşturma yetkisi." },
                    { new Guid("953cd531-6149-4e63-bd5d-862f41053d32"), "workspace.member.remove", "Çalışma alanından mevcut bir üyeyi tamamen çıkarma yetkisi." },
                    { new Guid("a2a7e721-5dc3-403a-bdd5-628887602839"), "taskitem.update", "Grup içindeki görevleri güncelleme yetkisi." },
                    { new Guid("af49de62-068e-467e-8f17-9ba4ccf33473"), "taskitem.create", "Grup içindeki görevleri oluşturma yetkisi." },
                    { new Guid("b0670926-df3f-41e3-a5d8-9a2cb70967e7"), "attachments.read", "Görevler için dosyaları görüntüleme yetkisi." },
                    { new Guid("b1caed1d-4b83-47d2-839d-dc5a9b9b3617"), "attachments.create", "Görevler için dosya ekleme yetkisi." },
                    { new Guid("c3e756b7-e7c8-4373-8741-a3328e5413dd"), "attachments.delete", "Görevler için dosyaları silme yetkisi." },
                    { new Guid("cd0c59f8-ab0f-45cc-97bc-98c6ac13344f"), "comments.update", "Görevler için yorumları güncelleme yetkisi." },
                    { new Guid("d5f4d12d-1533-4143-98f3-ef0f9d4c92ed"), "projects.delete", "Bir projeyi/panoyu içindeki tüm görevlerle birlikte kalıcı olarak silme yetkisi." },
                    { new Guid("de5201a9-501f-4b61-9bb5-afd6e7342c1a"), "workspace.update", "Çalışma alanının ismini, açıklamasını ve kurumsal genel ayarlarını düzenleme yetkisi." },
                    { new Guid("e885fd39-00d9-4a16-b94b-bcf65b60845b"), "workspace.role.delete", "Çalışma alanına özel oluşturulmuş rolleri tamamen silme yetkisi." },
                    { new Guid("f5a6d297-a279-4563-967a-f8cd37af9732"), "projects.update", "Mevcut projelerin isim, renk, kategori ve temel ayarlarını düzenleme yetkisi." },
                    { new Guid("ffc869ed-7030-4d5a-9eab-9957d13efe8c"), "attachments.update", "Görevler için dosyaları güncelleme yetkisi." }
                });

            migrationBuilder.InsertData(
                table: "WorkspaceRoles",
                columns: new[] { "Id", "Name", "WorkspaceId" },
                values: new object[,]
                {
                    { new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a"), "Owner", null },
                    { new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e"), "Administrator", null },
                    { new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c"), "Viewer", null },
                    { new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce"), "Member", null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("02b08e56-e99f-4ef3-a99e-8ec70754194f"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("0592df3c-6b6b-4b1d-bff2-5d779755a890"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("07a67e69-1d76-4ba4-a644-f04863a29c90"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("11afb428-b2d3-40d3-8008-b5c7aecba8b4"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("1a7b0835-2203-4305-b9bc-17633440d35c"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("23c879f0-49ca-402a-9610-574677f4f70d"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("31e13039-cb2f-497c-8fb9-1d98d480aa96"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("365ccfa9-d0ce-4a7a-8e07-a7eaa4238474"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("5193bc28-ea67-43d7-a105-7026d2d668e3"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("601059f0-0212-498f-9bf4-cdee28e4ba2c"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("6151859b-1c5c-4bd3-8202-656c02ae189d"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("66f5f8bc-a5cf-4851-8c01-79dd50cfa4b6"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("6b08ed62-b54e-41b5-8f10-0e52062150df"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("753469fe-c14a-4e8a-914e-786f3e78322d"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("8c9a2c14-6516-493e-9b21-56c113a86c13"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("912c74bc-4f5c-4f1c-8753-05cfad084bfd"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("93ecadf9-ce30-4843-ad61-236d39e7184b"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("953cd531-6149-4e63-bd5d-862f41053d32"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("a2a7e721-5dc3-403a-bdd5-628887602839"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("af49de62-068e-467e-8f17-9ba4ccf33473"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("b0670926-df3f-41e3-a5d8-9a2cb70967e7"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("b1caed1d-4b83-47d2-839d-dc5a9b9b3617"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("c3e756b7-e7c8-4373-8741-a3328e5413dd"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("cd0c59f8-ab0f-45cc-97bc-98c6ac13344f"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("d5f4d12d-1533-4143-98f3-ef0f9d4c92ed"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("de5201a9-501f-4b61-9bb5-afd6e7342c1a"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("e885fd39-00d9-4a16-b94b-bcf65b60845b"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("f5a6d297-a279-4563-967a-f8cd37af9732"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("ffc869ed-7030-4d5a-9eab-9957d13efe8c"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("02b08e56-e99f-4ef3-a99e-8ec70754194f"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("0592df3c-6b6b-4b1d-bff2-5d779755a890"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("07a67e69-1d76-4ba4-a644-f04863a29c90"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("11afb428-b2d3-40d3-8008-b5c7aecba8b4"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("23c879f0-49ca-402a-9610-574677f4f70d"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("31e13039-cb2f-497c-8fb9-1d98d480aa96"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("365ccfa9-d0ce-4a7a-8e07-a7eaa4238474"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("5193bc28-ea67-43d7-a105-7026d2d668e3"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("601059f0-0212-498f-9bf4-cdee28e4ba2c"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("6151859b-1c5c-4bd3-8202-656c02ae189d"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("66f5f8bc-a5cf-4851-8c01-79dd50cfa4b6"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("6b08ed62-b54e-41b5-8f10-0e52062150df"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("753469fe-c14a-4e8a-914e-786f3e78322d"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("8c9a2c14-6516-493e-9b21-56c113a86c13"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("912c74bc-4f5c-4f1c-8753-05cfad084bfd"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("93ecadf9-ce30-4843-ad61-236d39e7184b"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("953cd531-6149-4e63-bd5d-862f41053d32"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("a2a7e721-5dc3-403a-bdd5-628887602839"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("af49de62-068e-467e-8f17-9ba4ccf33473"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("b0670926-df3f-41e3-a5d8-9a2cb70967e7"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("b1caed1d-4b83-47d2-839d-dc5a9b9b3617"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("c3e756b7-e7c8-4373-8741-a3328e5413dd"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("cd0c59f8-ab0f-45cc-97bc-98c6ac13344f"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("d5f4d12d-1533-4143-98f3-ef0f9d4c92ed"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("de5201a9-501f-4b61-9bb5-afd6e7342c1a"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("e885fd39-00d9-4a16-b94b-bcf65b60845b"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("f5a6d297-a279-4563-967a-f8cd37af9732"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("ffc869ed-7030-4d5a-9eab-9957d13efe8c"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("07a67e69-1d76-4ba4-a644-f04863a29c90"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") },
                    { new Guid("11afb428-b2d3-40d3-8008-b5c7aecba8b4"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") },
                    { new Guid("66f5f8bc-a5cf-4851-8c01-79dd50cfa4b6"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") },
                    { new Guid("8c9a2c14-6516-493e-9b21-56c113a86c13"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") },
                    { new Guid("b0670926-df3f-41e3-a5d8-9a2cb70967e7"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") },
                    { new Guid("07a67e69-1d76-4ba4-a644-f04863a29c90"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("11afb428-b2d3-40d3-8008-b5c7aecba8b4"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("365ccfa9-d0ce-4a7a-8e07-a7eaa4238474"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("5193bc28-ea67-43d7-a105-7026d2d668e3"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("66f5f8bc-a5cf-4851-8c01-79dd50cfa4b6"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("8c9a2c14-6516-493e-9b21-56c113a86c13"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("93ecadf9-ce30-4843-ad61-236d39e7184b"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("a2a7e721-5dc3-403a-bdd5-628887602839"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("af49de62-068e-467e-8f17-9ba4ccf33473"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("b0670926-df3f-41e3-a5d8-9a2cb70967e7"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("b1caed1d-4b83-47d2-839d-dc5a9b9b3617"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("cd0c59f8-ab0f-45cc-97bc-98c6ac13344f"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") },
                    { new Guid("ffc869ed-7030-4d5a-9eab-9957d13efe8c"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssignedUserId",
                table: "TaskItems",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_WorkspaceId",
                table: "TaskItems",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspaceRoles_RoleId",
                table: "UserWorkspaceRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspaceRoles_WorkspaceId",
                table: "UserWorkspaceRoles",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceRoles_Name",
                table: "WorkspaceRoles",
                column: "Name",
                unique: true,
                filter: "\"WorkspaceId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceRoles_WorkspaceId_Name",
                table: "WorkspaceRoles",
                columns: new[] { "WorkspaceId", "Name" },
                unique: true,
                filter: "\"WorkspaceId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "UserWorkspaceRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkspaceRoles");

            migrationBuilder.DropTable(
                name: "Workspaces");
        }
    }
}
