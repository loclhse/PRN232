using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdAndIsCustomToGiftBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 27, 43, 55, DateTimeKind.Utc).AddTicks(2660));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 27, 43, 55, DateTimeKind.Utc).AddTicks(2659));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 27, 43, 55, DateTimeKind.Utc).AddTicks(2657));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 27, 43, 55, DateTimeKind.Utc).AddTicks(2653));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$rpby7JsI.bPCfKVyV2Gp0uflj.rBYqd6A0DvK3m87tA4bC5v/G8r.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9f8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$0sU/wDbbHRv4XwxKLLEyBeNH9JLtjDKs/0wwzj3MMRDy6rjUEBdWu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 10, 30, 556, DateTimeKind.Utc).AddTicks(4250));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 10, 30, 556, DateTimeKind.Utc).AddTicks(4239));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 10, 30, 556, DateTimeKind.Utc).AddTicks(4238));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 10, 30, 556, DateTimeKind.Utc).AddTicks(4233));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$HRomW744A4aBFWQpaP/fVOQdxN17BpuV9kNpFi1arFtglH7Tn4cMG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9f8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$UfW5gBNsrUeG2VZRTf9lDu31Fo6bHNMFPVrZ3SxBqsEt05MrCEVjW");
        }
    }
}
