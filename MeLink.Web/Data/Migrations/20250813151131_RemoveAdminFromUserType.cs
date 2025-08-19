using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeLink.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminFromUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Customer");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Pharmacy");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Medicine Store");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Pharmaceutical Company");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Manufacturer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Customer");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Pharmacy");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Medicine Store");

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Pharmaceutical Company");

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 6, null, "Manufacturer" });
        }
    }
}
