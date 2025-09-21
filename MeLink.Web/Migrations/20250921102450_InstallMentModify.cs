using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeLink.Web.Migrations
{
    /// <inheritdoc />
    public partial class InstallMentModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manufacturer_Installment",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MedicineWarehouse_Installment",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Pharmacy_Installment",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Manufacturer_Installment",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MedicineWarehouse_Installment",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pharmacy_Installment",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }
    }
}
