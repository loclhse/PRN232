using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftBoxUserAndIsCustomColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột IsCustom và UserId cho bảng GiftBoxes
            migrationBuilder.AddColumn<bool>(
                name: "IsCustom",
                table: "GiftBoxes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "GiftBoxes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiftBoxes_UserId",
                table: "GiftBoxes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiftBoxes_Users_UserId",
                table: "GiftBoxes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Cập nhật seed dữ liệu Roles / Users
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 37, 43, 968, DateTimeKind.Utc).AddTicks(5049));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 37, 43, 968, DateTimeKind.Utc).AddTicks(5038));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 37, 43, 968, DateTimeKind.Utc).AddTicks(5036));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 6, 37, 43, 968, DateTimeKind.Utc).AddTicks(5032));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$tDj2OYL3tGkgjCYftZmodei95OP2ewLGqF.7Pp6bieYNQt48hz.fi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9f8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$yvFqh.32cBsl0kQquoOGI.n1CLcBnOpravj.lu9D/raYrOwwhU1PW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Gỡ FK và cột UserId, IsCustom trên GiftBoxes
            migrationBuilder.DropForeignKey(
                name: "FK_GiftBoxes_Users_UserId",
                table: "GiftBoxes");

            migrationBuilder.DropIndex(
                name: "IX_GiftBoxes_UserId",
                table: "GiftBoxes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GiftBoxes");

            migrationBuilder.DropColumn(
                name: "IsCustom",
                table: "GiftBoxes");

            // Revert lại seed dữ liệu Roles / Users
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
    }
}
