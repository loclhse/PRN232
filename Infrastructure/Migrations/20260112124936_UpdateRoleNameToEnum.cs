using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleNameToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("7c8d9e0f-1a2b-3c4d-5e6f-7a8b9c0d1e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 49, 35, 760, DateTimeKind.Utc).AddTicks(4749));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 49, 35, 760, DateTimeKind.Utc).AddTicks(4593));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 49, 35, 760, DateTimeKind.Utc).AddTicks(4592));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 49, 35, 760, DateTimeKind.Utc).AddTicks(4590));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 49, 35, 760, DateTimeKind.Utc).AddTicks(4586));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$rqmGJ1fxzx61I8/UWiPiieIVVF16AmWR5zORk7ohQwbMvI7MbrRSS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$11$8ujYInRg5V3Cr32kPT61QeuOduILqJolwbNHId.RmzYL3o7/pYxfW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("7c8d9e0f-1a2b-3c4d-5e6f-7a8b9c0d1e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 37, 3, 476, DateTimeKind.Utc).AddTicks(631));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 37, 3, 476, DateTimeKind.Utc).AddTicks(459));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 37, 3, 476, DateTimeKind.Utc).AddTicks(457));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 37, 3, 476, DateTimeKind.Utc).AddTicks(455));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 12, 37, 3, 476, DateTimeKind.Utc).AddTicks(451));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$Pc88/TZCdztUvcl7no.yO.dEyVNhY/eD.lS36ZMbX/8aimtRrbF4G");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$11$a5bLvLv1GFgRTfjbCWXZL.HpXQzSI6nSA3UcXX/6vP.fcWdJd/E5q");
        }
    }
}
