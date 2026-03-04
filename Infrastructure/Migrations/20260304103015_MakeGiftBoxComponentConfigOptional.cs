using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeGiftBoxComponentConfigOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid?>(
                name: "GiftBoxComponentConfigId",
                table: "GiftBoxes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "GiftBoxComponentConfigId",
                table: "GiftBoxes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid?),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
