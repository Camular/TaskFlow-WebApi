using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RefineRbacModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Workspaces_WorkspaceId",
                table: "TaskItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkspaceId",
                table: "WorkspaceRoles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkspaceId",
                table: "TaskItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserWorkspaceRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkspaceRoles", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceRoles_Name",
                table: "WorkspaceRoles",
                column: "Name",
                unique: true,
                filter: "\"WorkspaceId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceRoles_Name_WorkspaceId",
                table: "WorkspaceRoles",
                columns: new[] { "Name", "WorkspaceId" },
                unique: true,
                filter: "\"WorkspaceId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceRoles_WorkspaceId",
                table: "WorkspaceRoles",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspaceRoles_RoleId",
                table: "UserWorkspaceRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspaceRoles_UserId_RoleId_WorkspaceId",
                table: "UserWorkspaceRoles",
                columns: new[] { "UserId", "RoleId", "WorkspaceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspaceRoles_WorkspaceId",
                table: "UserWorkspaceRoles",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_WorkspaceRoles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "WorkspaceRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Workspaces_WorkspaceId",
                table: "TaskItems",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceRoles_Workspaces_WorkspaceId",
                table: "WorkspaceRoles",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_WorkspaceRoles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Workspaces_WorkspaceId",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceRoles_Workspaces_WorkspaceId",
                table: "WorkspaceRoles");

            migrationBuilder.DropTable(
                name: "UserWorkspaceRoles");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceRoles_Name",
                table: "WorkspaceRoles");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceRoles_Name_WorkspaceId",
                table: "WorkspaceRoles");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceRoles_WorkspaceId",
                table: "WorkspaceRoles");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkspaceId",
                table: "WorkspaceRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkspaceId",
                table: "TaskItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Workspaces_WorkspaceId",
                table: "TaskItems",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id");
        }
    }
}
