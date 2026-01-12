using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleAndCleanUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 9, 43, 42, 958, DateTimeKind.Utc).AddTicks(7465));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 9, 43, 42, 958, DateTimeKind.Utc).AddTicks(7230));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 9, 43, 42, 958, DateTimeKind.Utc).AddTicks(7235));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 9, 43, 42, 958, DateTimeKind.Utc).AddTicks(7236));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 9, 43, 42, 958, DateTimeKind.Utc).AddTicks(7237));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$zQpgTkxHcdrD/3N9VLQj7erSeqXjGAEETDO77plpJKG0PF7mOEao2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$SO4h.XEuij9qHS4CoruazuGuffCkT9ty2Gvncg8kPxh1.bCdcwW/q");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 7, 21, 33, 436, DateTimeKind.Utc).AddTicks(7150));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 7, 21, 33, 436, DateTimeKind.Utc).AddTicks(6979));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 7, 21, 33, 436, DateTimeKind.Utc).AddTicks(6982));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 7, 21, 33, 436, DateTimeKind.Utc).AddTicks(6983));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 12, 7, 21, 33, 436, DateTimeKind.Utc).AddTicks(6984));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$N2/xDYuA7HuVuOvnL4VJCunAaZq5Fas1oxLSGrfbdILlUqjnnzpDq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$/5aUvzY7KzdThMb4Rsf6I.1nXEf9smv1YawVzCO9LbPGXDEDXWY5e");
        }
    }
}
