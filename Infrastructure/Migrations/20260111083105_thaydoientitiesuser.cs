using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class thaydoientitiesuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpExpiryTime",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserOtp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    OtpType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOtp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOtp_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 8, 31, 4, 210, DateTimeKind.Utc).AddTicks(1391));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 8, 31, 4, 210, DateTimeKind.Utc).AddTicks(1234));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 8, 31, 4, 210, DateTimeKind.Utc).AddTicks(1237));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 8, 31, 4, 210, DateTimeKind.Utc).AddTicks(1238));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 11, 8, 31, 4, 210, DateTimeKind.Utc).AddTicks(1239));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$dsyxDRflSSA8QWjB49Fbz.OTrxNnX1usiVzDdmipYxnOX4JPIJAtq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$96vqzkhf9ZY85fPd.2.5cOCfRaoIbU8WBTAp8JkcQyNOXWNeYs0eO");

            migrationBuilder.CreateIndex(
                name: "IX_UserOtp_UserId",
                table: "UserOtp",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOtp");

            migrationBuilder.AddColumn<string>(
                name: "OtpCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiryTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 52, 28, 164, DateTimeKind.Utc).AddTicks(3077));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 52, 28, 164, DateTimeKind.Utc).AddTicks(2873));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 52, 28, 164, DateTimeKind.Utc).AddTicks(2876));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 52, 28, 164, DateTimeKind.Utc).AddTicks(2920));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 10, 15, 52, 28, 164, DateTimeKind.Utc).AddTicks(2921));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "OtpCode", "OtpExpiryTime", "PasswordHash" },
                values: new object[] { null, null, "$2a$11$HWsgxnb3NrP7QZOSaeTdWutoBprGOGDS0dFDbjHK1.OBb2cF4FplS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "OtpCode", "OtpExpiryTime", "PasswordHash" },
                values: new object[] { null, null, "$2a$11$//rRQRZBcaaHdBiYZ9Xl2.bAfkcn5vs7HraMB9IUvZs98FqW5z7gC" });
        }
    }
}
