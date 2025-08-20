using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeLink.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class inventoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Inventories",
                newName: "StockQuantity");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Inventories",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Inventories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Inventories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryAvailable",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "IsDeliveryAvailable",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "Inventories",
                newName: "Quantity");
        }
    }
}
