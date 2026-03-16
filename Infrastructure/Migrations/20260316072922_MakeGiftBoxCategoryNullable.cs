using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeGiftBoxCategoryNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "GiftBoxes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 16, 7, 29, 21, 558, DateTimeKind.Utc).AddTicks(647));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 16, 7, 29, 21, 558, DateTimeKind.Utc).AddTicks(646));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 16, 7, 29, 21, 558, DateTimeKind.Utc).AddTicks(644));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 16, 7, 29, 21, 558, DateTimeKind.Utc).AddTicks(641));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$12$qY4PyuQWU3I7BV/3lgEg.eFNLY6lBRnUG1QOWwOXUpMhFrD7EMo0O");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$12$qY4PyuQWU3I7BV/3lgEg.eFNLY6lBRnUG1QOWwOXUpMhFrD7EMo0O");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "GiftBoxes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 17, 15, 16, 313, DateTimeKind.Utc).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 17, 15, 16, 313, DateTimeKind.Utc).AddTicks(1219));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 17, 15, 16, 313, DateTimeKind.Utc).AddTicks(1217));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 13, 17, 15, 16, 313, DateTimeKind.Utc).AddTicks(1212));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$vDFqbKnY3QL4ORCoM43bNuP/sRM0hjmcCMIdNwSj2Uc2249l4Zq56");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$11$Q/0mlgudB04akFd9ikcPROVz8jC6f8MoeWxo2Yt3yhd7O88BDGSC.");
        }
    }
}
