using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingPhone",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 56, 24, 197, DateTimeKind.Utc).AddTicks(1489));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 56, 24, 197, DateTimeKind.Utc).AddTicks(1488));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 56, 24, 197, DateTimeKind.Utc).AddTicks(1486));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 26, 12, 56, 24, 197, DateTimeKind.Utc).AddTicks(1482));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$DgnInQ3PCleuaDjTZ4KxGOP8F0sUMJTKyFmNMD4xETFoccWlG58OS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$11$SyErOtigxDuHwAuH2KNP8eih0HJOV.4Iji95wq74hjLb4ycC83Wn.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingPhone",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 3, 0, 42, 30, 66, DateTimeKind.Utc).AddTicks(6181));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 3, 0, 42, 30, 66, DateTimeKind.Utc).AddTicks(6180));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 3, 0, 42, 30, 66, DateTimeKind.Utc).AddTicks(6178));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 2, 3, 0, 42, 30, 66, DateTimeKind.Utc).AddTicks(6174));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$SrCgv414CUhk5981tHXzHO10HjV9PClE.MrF0QpzaRXU4kXhaOEpa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$11$5zjt22ugXzlAnWxKSyqoPO2PgXp5YLbi4wqCxDPpKuKx9WMIbOr4.");
        }
    }
}
