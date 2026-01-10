using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToOtpFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResetTokenExpiryTime",
                table: "Users",
                newName: "OtpExpiryTime");

            migrationBuilder.RenameColumn(
                name: "PasswordResetToken",
                table: "Users",
                newName: "OtpCode");

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 13, 12, 46, 742, DateTimeKind.Utc).AddTicks(9948));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 13, 12, 46, 742, DateTimeKind.Utc).AddTicks(9784));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 13, 12, 46, 742, DateTimeKind.Utc).AddTicks(9788));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 13, 12, 46, 742, DateTimeKind.Utc).AddTicks(9789));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 13, 12, 46, 742, DateTimeKind.Utc).AddTicks(9790));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$X8o8g2Vx65CdDxzvhFQ/1emWG9VdLicNv4EFfkN8suJboAaIrNl12");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$6inOKdnOPy/TKOjtwQjHc.iDFpFqvZStg47c.JHA1ZNZ58PMBsK9u");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtpExpiryTime",
                table: "Users",
                newName: "ResetTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "OtpCode",
                table: "Users",
                newName: "PasswordResetToken");

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 12, 13, 6, 903, DateTimeKind.Utc).AddTicks(5666));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 12, 13, 6, 903, DateTimeKind.Utc).AddTicks(5500));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 12, 13, 6, 903, DateTimeKind.Utc).AddTicks(5503));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 12, 13, 6, 903, DateTimeKind.Utc).AddTicks(5504));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 12, 13, 6, 903, DateTimeKind.Utc).AddTicks(5505));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$hnfxS6MuWkwGM9yaHMVeKeUmK4k.CrpLdsdxYsiwkD03tEEuBrz5S");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$J..JuRZ5z81M8osUj1I/KuLtzurjENOXfHtFaXawOqhI.kOs05kwa");
        }
    }
}
