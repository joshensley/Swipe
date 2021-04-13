using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class EditPassModelInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationUserLikedID",
                table: "Pass",
                newName: "ApplicationUserPassID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationUserPassID",
                table: "Pass",
                newName: "ApplicationUserLikedID");
        }
    }
}
