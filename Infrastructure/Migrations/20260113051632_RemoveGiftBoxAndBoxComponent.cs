using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGiftBoxAndBoxComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_GiftBoxes_GiftBoxId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_GiftBoxes_ItemId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ItemId",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "BoxComponents");

            migrationBuilder.DropTable(
                name: "GiftBoxes");

            migrationBuilder.DropIndex(
                name: "IX_Images_GiftBoxId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "GiftBoxId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "OrderDetails",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ItemId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId");

            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("7c8d9e0f-1a2b-3c4d-5e6f-7a8b9c0d1e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 13, 5, 16, 31, 281, DateTimeKind.Utc).AddTicks(5983));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 13, 5, 16, 31, 281, DateTimeKind.Utc).AddTicks(5832));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 13, 5, 16, 31, 281, DateTimeKind.Utc).AddTicks(5830));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 13, 5, 16, 31, 281, DateTimeKind.Utc).AddTicks(5828));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b"),
                column: "CreatedAt",
                value: new DateTime(2026, 1, 13, 5, 16, 31, 281, DateTimeKind.Utc).AddTicks(5824));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d8c7b6-a5b4-4c3d-2e1f-0a1b2c3d4e5f"),
                column: "PasswordHash",
                value: "$2a$11$.LH0t7k80as7ku3JFY5cC.b9MLMt8T5LOtzNYqNY8KLd.Ozfs7sYS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c"),
                column: "PasswordHash",
                value: "$2a$11$UrKux23i/6gopPR6liJzw.4G.gjbj4myUODKbYSdPLhTQ3OrorNri");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderDetails",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ItemId");

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "OrderDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GiftBoxId",
                table: "Images",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GiftBoxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftBoxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftBoxes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoxComponents",
                columns: table => new
                {
                    GiftBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxComponents", x => new { x.GiftBoxId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_BoxComponents_GiftBoxes_GiftBoxId",
                        column: x => x.GiftBoxId,
                        principalTable: "GiftBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoxComponents_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Images_GiftBoxId",
                table: "Images",
                column: "GiftBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxComponents_ProductId",
                table: "BoxComponents",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftBoxes_CategoryId",
                table: "GiftBoxes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_GiftBoxes_GiftBoxId",
                table: "Images",
                column: "GiftBoxId",
                principalTable: "GiftBoxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_GiftBoxes_ItemId",
                table: "OrderDetails",
                column: "ItemId",
                principalTable: "GiftBoxes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ItemId",
                table: "OrderDetails",
                column: "ItemId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
