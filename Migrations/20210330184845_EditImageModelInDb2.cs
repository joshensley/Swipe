using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class EditImageModelInDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_AspNetUsers_ApplicationUserIDId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserIDId",
                table: "Images",
                newName: "ApplicationUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ApplicationUserIDId",
                table: "Images",
                newName: "IX_Images_ApplicationUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_AspNetUsers_ApplicationUserID",
                table: "Images",
                column: "ApplicationUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_AspNetUsers_ApplicationUserID",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserID",
                table: "Images",
                newName: "ApplicationUserIDId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ApplicationUserID",
                table: "Images",
                newName: "IX_Images_ApplicationUserIDId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_AspNetUsers_ApplicationUserIDId",
                table: "Images",
                column: "ApplicationUserIDId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
