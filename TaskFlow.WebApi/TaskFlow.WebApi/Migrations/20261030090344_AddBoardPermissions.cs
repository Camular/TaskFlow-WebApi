using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskFlow.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("12d9f34f-4323-4bd8-aef9-59d46afb3474"), "boards.delete", "Delete boards and columns in the workspace." },
                    { new Guid("302fc825-1f95-4628-9a2d-38a911a19229"), "boards.update", "Update boards and columns in the workspace." },
                    { new Guid("bbf12eeb-b444-4211-9313-7e69c0272fdc"), "boards.create", "Create boards and columns in the workspace." },
                    { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), "boards.read", "View boards and columns in the workspace." }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("12d9f34f-4323-4bd8-aef9-59d46afb3474"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("302fc825-1f95-4628-9a2d-38a911a19229"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("bbf12eeb-b444-4211-9313-7e69c0272fdc"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") },
                    { new Guid("12d9f34f-4323-4bd8-aef9-59d46afb3474"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("302fc825-1f95-4628-9a2d-38a911a19229"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("bbf12eeb-b444-4211-9313-7e69c0272fdc"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") },
                    { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") },
                    { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12d9f34f-4323-4bd8-aef9-59d46afb3474"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("302fc825-1f95-4628-9a2d-38a911a19229"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("bbf12eeb-b444-4211-9313-7e69c0272fdc"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("87f54898-67b8-42c9-b3bf-df33cd1d0c6a") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12d9f34f-4323-4bd8-aef9-59d46afb3474"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("302fc825-1f95-4628-9a2d-38a911a19229"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("bbf12eeb-b444-4211-9313-7e69c0272fdc"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("c89567a9-7c63-4c62-80de-dd54fc3a1c9e") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("d191a4b1-1ce3-43ed-8efa-3439ce5d6a9c") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"), new Guid("ecb6c928-cc9b-404d-b04b-a9612c377cce") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12d9f34f-4323-4bd8-aef9-59d46afb3474"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("302fc825-1f95-4628-9a2d-38a911a19229"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("bbf12eeb-b444-4211-9313-7e69c0272fdc"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c1a72273-8ad9-45fa-9cdb-4e55008dd3bf"));
        }
    }
}
