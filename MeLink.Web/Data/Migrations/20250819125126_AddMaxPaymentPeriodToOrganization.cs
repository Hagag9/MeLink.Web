using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeLink.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxPaymentPeriodToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Manufacturer_MaxPaymentPeriodInDays",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPaymentPeriodInDays",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicineWarehouse_MaxPaymentPeriodInDays",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserRelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRelation_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRelation_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRelation_FromUserId",
                table: "UserRelation",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelation_ToUserId",
                table: "UserRelation",
                column: "ToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRelation");

            migrationBuilder.DropColumn(
                name: "Manufacturer_MaxPaymentPeriodInDays",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxPaymentPeriodInDays",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MedicineWarehouse_MaxPaymentPeriodInDays",
                table: "AspNetUsers");
        }
    }
}
