using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeLink.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRelation_AspNetUsers_FromUserId",
                table: "UserRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRelation_AspNetUsers_ToUserId",
                table: "UserRelation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRelation",
                table: "UserRelation");

            migrationBuilder.RenameTable(
                name: "UserRelation",
                newName: "UserRelations");

            migrationBuilder.RenameIndex(
                name: "IX_UserRelation_ToUserId",
                table: "UserRelations",
                newName: "IX_UserRelations_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRelation_FromUserId",
                table: "UserRelations",
                newName: "IX_UserRelations_FromUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRelations",
                table: "UserRelations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelations_AspNetUsers_FromUserId",
                table: "UserRelations",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelations_AspNetUsers_ToUserId",
                table: "UserRelations",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRelations_AspNetUsers_FromUserId",
                table: "UserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRelations_AspNetUsers_ToUserId",
                table: "UserRelations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRelations",
                table: "UserRelations");

            migrationBuilder.RenameTable(
                name: "UserRelations",
                newName: "UserRelation");

            migrationBuilder.RenameIndex(
                name: "IX_UserRelations_ToUserId",
                table: "UserRelation",
                newName: "IX_UserRelation_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRelations_FromUserId",
                table: "UserRelation",
                newName: "IX_UserRelation_FromUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRelation",
                table: "UserRelation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelation_AspNetUsers_FromUserId",
                table: "UserRelation",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelation_AspNetUsers_ToUserId",
                table: "UserRelation",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
